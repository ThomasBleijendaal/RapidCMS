﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Forms
{
    internal class FormState
    {
        private readonly List<string> _messages = new List<string>();
        private readonly List<PropertyState> _fieldStates = new List<PropertyState>();
        private readonly IEntity _entity;
        private readonly IServiceProvider _serviceProvider;

        public FormState(IEntity entity, IServiceProvider serviceProvider)
        {
            _entity = entity;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<string> GetValidationMessages()
        {
            foreach (var message in _messages)
            {
                yield return message;
            }
            foreach (var message in _fieldStates.SelectMany(x => x.GetValidationMessages()))
            {
                yield return message;
            }
        }

        public IEnumerable<string> GetStrayValidationMessages()
        {
            return _messages;
        }

        public void AddMessage(string message)
        {
            _messages.Add(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
            foreach (var fieldState in _fieldStates)
            {
                fieldState.ClearMessages();
            }
        }

        public ModelStateDictionary ModelState
        {
            get
            {
                var state = new ModelStateDictionary();

                _fieldStates.ForEach(fs => fs.GetValidationMessages().ForEach(message => state.Add(fs.Property.PropertyName, message)));
                _messages.ForEach(m => state.Add(string.Empty, m));

                return state;
            }
        }

        public void PopulatePropertyStatesUsingReferenceEntity(IEntity reference)
        {
            GetPropertyMetadatas(reference).ForEach(property =>
            {
                if ((property.PropertyType.IsValueType || 
                    property.PropertyType == typeof(string)) && 
                    !Equals(property.Getter(reference), property.Getter(_entity)))
                {
                    GetPropertyState(property)!.IsModified = true;
                }
            });
        }

        public void PopulateAllPropertyStates()
        {
            GetPropertyMetadatas(_entity).ForEach(property => GetPropertyState(property, createWhenNotFound: true));
        }

        private IEnumerable<IPropertyMetadata> GetPropertyMetadatas(IEntity reference, IEnumerable<PropertyInfo>? objectGetters = default)
        {
            Func<object, object> getObject;
            if (objectGetters == null)
            {
                getObject = (root) => root;
            }
            else
            {
                getObject = (root) => objectGetters.Aggregate(root, (@obj, objectGetter) => objectGetter.GetValue(@obj)!);
            }

            var properties = getObject(reference).GetType().GetProperties();

            foreach (var property in properties)
            {
                var validateObjectAttribute = property.GetCustomAttribute<ValidateObjectAttribute>();
                if (validateObjectAttribute != null)
                {
                    // only venture into nested objects when the model wants them validated
                    foreach (var nestedPropertyMetadata in GetPropertyMetadatas(reference, (objectGetters ?? new PropertyInfo[] { }).Union(new[] { property })))
                    {
                        yield return nestedPropertyMetadata;
                    }
                }

                var propertyMetadata = PropertyMetadataHelper.GetPropertyMetadata(reference.GetType(), objectGetters, property);
                if (propertyMetadata == null)
                {
                    continue;
                }

                yield return propertyMetadata;
            }
        }

        public IEnumerable<PropertyState> GetPropertyStates()
        {
            return _fieldStates;
        }

        public PropertyState? GetPropertyState(string propertyName)
        {
            return _fieldStates.SingleOrDefault(field => field.Property.PropertyName == propertyName);
        }

        public PropertyState? GetPropertyState(IPropertyMetadata property, bool createWhenNotFound = true)
        {
            var fieldState = _fieldStates.SingleOrDefault(x => x.Property.Fingerprint == property.Fingerprint);
            if (fieldState == null)
            {
                if (!createWhenNotFound)
                {
                    return default;
                }

                fieldState = new PropertyState(property);
                _fieldStates.Add(fieldState);
            }

            return fieldState;
        }

        public void ValidateModel(IEnumerable<IDataValidationProvider>? dataProviders)
        {
            var results = GetValidationResultsForModel(dataProviders);

            foreach (var result in results)
            {
                var strayError = true;
                result.MemberNames.ForEach(name =>
                {
                    GetPropertyState(name)?.AddMessage(result.ErrorMessage ?? "Unknown error");
                    strayError = false;
                });

                if (strayError)
                {
                    _messages.Add(result.ErrorMessage ?? "Unknown error");
                }
            }

            _fieldStates.ForEach(x => x.WasValidated = true);
        }

        public void ValidateProperty(IPropertyMetadata property, IEnumerable<IDataValidationProvider>? dataProviders)
        {
            IEnumerable<ValidationResult> results;

            // when a property must be validated which is not part of the entity (like a nested object)
            // validate the complete model, but only keep the results of that property
            if (_entity.GetType().GetProperty(property.PropertyName) == null)
            {
                results = GetValidationResultsForModel(dataProviders).Where(x => x.MemberNames.Contains(property.PropertyName));
            }
            else
            {
                results = GetValidationResultsForProperty(property, dataProviders);
            }

            var state = GetPropertyState(property)!;
            state.ClearMessages(clearManualMessages: true);
            state.WasValidated = true;

            foreach (var result in results)
            {
                state.AddMessage(result.ErrorMessage ?? "Unknown error");
            }
        }

        private IEnumerable<ValidationResult> GetValidationResultsForProperty(IPropertyMetadata property, IEnumerable<IDataValidationProvider>? dataProviders)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(_entity, _serviceProvider, null)
            {
                MemberName = property.PropertyName
            };

            try
            {
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateProperty(property.Getter(_entity), context, results);
            }
            catch { }

            if (dataProviders != null)
            {
                foreach (var result in dataProviders.Where(p => p.Property == property).SelectMany(p => p.Validate(_entity, _serviceProvider)))
                {
                    results.Add(result);
                }
            }

            return results;
        }

        private IEnumerable<ValidationResult> GetValidationResultsForModel(IEnumerable<IDataValidationProvider>? dataProviders)
        {
            var context = new ValidationContext(_entity, _serviceProvider, null);
            var results = new List<ValidationResult>();

            try
            {
                // even though this says Try, and therefore it should not throw an error, IT DOES when a given property is not part of Entity
                Validator.TryValidateObject(_entity, context, results, true);
            }
            catch { }

            ClearMessages();

            _fieldStates
                .Where(kv => kv.IsBusy)
                .ForEach(kv => results.Add(new ValidationResult(
                    $"The {kv.Property.PropertyName} field indicates it is performing an asynchronous task which must be awaited.",
                    new[] { kv.Property.PropertyName })));

            if (dataProviders != null)
            {
                foreach (var result in dataProviders.SelectMany(p => p.Validate(_entity, _serviceProvider)))
                {
                    results.Add(result);
                }
            }

            return FlattenCompositeValidationResults(results);
        }

        private IEnumerable<ValidationResult> FlattenCompositeValidationResults(IEnumerable<ValidationResult> results, string? memberNamePrefix = null)
        {
            foreach (var result in results)
            {
                if (result is CompositeValidationResult composite)
                {
                    foreach (var nestedResult in FlattenCompositeValidationResults(composite.Results, composite.MemberName))
                    {
                        yield return nestedResult;
                    }
                }
                else if (string.IsNullOrWhiteSpace(memberNamePrefix))
                {
                    yield return result;
                }
                else
                {
                    yield return new ValidationResult(result.ErrorMessage, result.MemberNames.Select(x => $"{memberNamePrefix}.{x}"));
                }
            }
        }
    }
}

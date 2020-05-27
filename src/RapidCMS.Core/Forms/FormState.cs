using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.Core.Providers;

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

        // TODO
        internal List<DataProvider> DataProviders = new List<DataProvider>();

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

        public void ValidateModel()
        {
            var results = GetValidationResultsForModel();

            foreach (var result in results)
            {
                var strayError = true;

                result.MemberNames.ForEach(name =>
                {
                    GetPropertyState(name)?.AddMessage(result.ErrorMessage);
                    strayError = false;
                });

                if (strayError)
                {
                    _messages.Add(result.ErrorMessage);
                }
            }

            _fieldStates.ForEach(x => x.WasValidated = true);
        }

        public void ValidateProperty(IPropertyMetadata property)
        {
            IEnumerable<ValidationResult> results;

            // when a property must be validated which is not part of the entity (like a nested object)
            // validate the complete model, but only keep the results of that property
            if (_entity.GetType().GetProperty(property.PropertyName) == null)
            {
                results = GetValidationResultsForModel().Where(x => x.MemberNames.Contains(property.PropertyName));
            }
            else
            {
                results = GetValidationResultsForProperty(property);
            }

            var state = GetPropertyState(property)!;
            state.ClearMessages();
            state.WasValidated = true;

            foreach (var result in results)
            {
                state.AddMessage(result.ErrorMessage);
            }
        }

        private IEnumerable<ValidationResult> GetValidationResultsForProperty(IPropertyMetadata property)
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
                Validator.TryValidateProperty(property.Getter(_entity), context, results);
            }
            catch { }

            foreach (var result in DataProviders.Where(p => p.Property == property).SelectMany(p => p.Validate(_entity, _serviceProvider)))
            {
                results.Add(result);
            }

            return results;
        }

        private IEnumerable<ValidationResult> GetValidationResultsForModel()
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

            foreach (var result in DataProviders.SelectMany(p => p.Validate(_entity, _serviceProvider)))
            {
                results.Add(result);
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
                    yield return new ValidationResult(result.ErrorMessage, result.MemberNames.Select(x => $"{memberNamePrefix}{x}"));
                }
            }
        }
    }
}

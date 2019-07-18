using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;
using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Common.Data
{
    [Obsolete("This thing is weird, requires refactor")]
    internal class DataContext
    {
        private readonly Dictionary<IPropertyMetadata, IDataCollection>? _dataCollections;
        private readonly Dictionary<IPropertyMetadata, IRelationValidator?>? _validators;

        private IEnumerable<KeyValuePair<IPropertyMetadata, IRelationDataCollection>> _relationDataCollections => 
            _dataCollections?
                .Where(x => x.Value is IRelationDataCollection)
                .Select(x => new KeyValuePair<IPropertyMetadata, IRelationDataCollection>(x.Key, (IRelationDataCollection)x.Value))
                ?? throw new InvalidOperationException("Incorrect DataContext");

        internal DataContext(List<Pane>? panes, IServiceProvider serviceProvider)
        {
            var configuration = panes.GetDataCollections(serviceProvider);

            _dataCollections = configuration.ToDictionary(x => x.property, x => x.relation);
            _validators = configuration.ToDictionary(x => x.property, x => x.validator);
        }

        //internal DataContext(ListEditor config, IServiceProvider serviceProvider)
        //{
        //    var configuration = config.GetDataCollections(serviceProvider);

        //    _dataCollections = configuration.ToDictionary(x => x.property, x => x.relation);
        //    _validators = configuration.ToDictionary(x => x.property, x => x.validator);
        //}

        //internal DataContext(ListView config, IServiceProvider serviceProvider)
        //{
        //    //var configuration = config.GetDataCollections(serviceProvider);

        //    //_dataCollections = configuration.ToDictionary(x => x.property, x => x.relation);
        //    //_validators = configuration.ToDictionary(x => x.property, x => x.validator);
        //}

        internal IDataCollection GetDataCollection(IPropertyMetadata propertyMetadata)
        {
            return _dataCollections?[propertyMetadata] ?? throw new InvalidOperationException("Incorrect DataContext");
        }

        internal IEnumerable<ValidationResult> ValidateRelations(IEntity entity)
        {
            foreach (var kv in _relationDataCollections)
            {
                var relation = GetRelation(kv);

                if (_validators?[kv.Key] is IRelationValidator validator)
                {
                    foreach (var result in validator.Validate(entity, relation.RelatedElements))
                    {
                        yield return result;
                    }
                }
            }
        }

        internal IEnumerable<ValidationResult> ValidateRelation(IEntity entity, IPropertyMetadata property)
        {
            var kv = _relationDataCollections.FirstOrDefault(x => x.Key == property);

            if (kv.Value != null)
            {
                var relation = GetRelation(kv);

                if (_validators?[property] is IRelationValidator validator)
                {
                    foreach (var result in validator.Validate(entity, relation.RelatedElements))
                    {
                        yield return result;
                    }
                }
            }
        }

        internal IRelationContainer? GenerateRelationContainer()
        {
            return (_dataCollections == null) 
                ? default 
                : new RelationContainer(_relationDataCollections.Select(x => GetRelation(x)));
        }

        private IRelation GetRelation(KeyValuePair<IPropertyMetadata, IRelationDataCollection> kv)
        {
            return new Relation(
                kv.Value.GetRelatedEntityType(),
                kv.Key,
                kv.Value.GetCurrentRelatedElements().Select(x => new RelatedElement { Id = x.Id }));
        }
    }
}

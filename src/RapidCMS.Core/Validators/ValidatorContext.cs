using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Validators
{
    public class ValidatorContext : IValidatorContext
    {
        public ValidatorContext(IEntity entity, IRelationContainer relationContainer, object? configuration)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            RelationContainer = relationContainer ?? throw new ArgumentNullException(nameof(relationContainer));
            Configuration = configuration;
        }

        public IEntity Entity { get; }

        public IRelationContainer RelationContainer { get; }

        public object? Configuration { get; }
    }

    public class ValidatorContext<TEntity> : IValidatorContext<TEntity>
    {
        public ValidatorContext(IEntity entity, IRelationContainer relationContainer, object? configuration)
        {
            Entity = (TEntity)entity ?? throw new ArgumentNullException(nameof(entity));
            RelationContainer = relationContainer ?? throw new ArgumentNullException(nameof(relationContainer));
            Configuration = configuration;
        }

        public TEntity Entity { get; }

        public IRelationContainer RelationContainer { get; }

        public object? Configuration { get; }
    }
}

using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Abstractions.Validators
{
    public interface IValidatorContext
    {
        IEntity Entity { get; }
        IRelationContainer RelationContainer { get; }
        object? Configuration { get; }
    }

    public interface IValidatorContext<TEntity>
    {
        TEntity Entity { get; }
        IRelationContainer RelationContainer { get; }
        object? Configuration { get; }
    }
}

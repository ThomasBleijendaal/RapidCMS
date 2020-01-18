using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    public class RelatedEntity : IRelated
    {
        public RelatedEntity(IEntity relatedEntity)
        {
            Entity = relatedEntity;
        }

        public IEntity Entity { get; private set; }
    }
}

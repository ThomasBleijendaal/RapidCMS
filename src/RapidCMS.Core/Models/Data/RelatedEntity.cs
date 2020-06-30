using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Data
{
    public class RelatedEntity : IRelated
    {
        public RelatedEntity(IEntity relatedEntity, string collectionAlias)
        {
            Entity = relatedEntity;
            CollectionAlias = collectionAlias;
        }

        public RelatedEntity(EditContext editContext) : this(editContext.Entity, editContext.CollectionAlias)
        {

        }

        public IEntity Entity { get; private set; }
        public string CollectionAlias { get; private set; }
    }
}

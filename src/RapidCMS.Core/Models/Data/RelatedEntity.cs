using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Data
{
    public class RelatedEntity : IRelated
    {
        public RelatedEntity(IEntity relatedEntity, string repositoryAlias)
        {
            Entity = relatedEntity;
            RepositoryAlias = repositoryAlias;
        }

        public RelatedEntity(EditContext editContext) : this(editContext.Entity, editContext.RepositoryAlias)
        {

        }

        public IEntity Entity { get; private set; }
        public string RepositoryAlias { get; private set; }
    }
}

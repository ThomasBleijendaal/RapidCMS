using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Data
{
    public class RelatedEntity : IRelated
    {
        public RelatedEntity(IParent? parent, IEntity relatedEntity, string repositoryAlias)
        {
            Parent = parent;
            Entity = relatedEntity;
            RepositoryAlias = repositoryAlias;
        }

        public RelatedEntity(FormEditContext editContext) : this(editContext.Parent, editContext.Entity, editContext.RepositoryAlias)
        {

        }

        public IParent? Parent { get; private set; }
        public IEntity Entity { get; private set; }
        public string RepositoryAlias { get; private set; }
    }
}

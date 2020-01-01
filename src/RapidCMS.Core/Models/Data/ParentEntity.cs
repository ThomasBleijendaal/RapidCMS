using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    internal class ParentEntity : IParent
    {
        private readonly string _collectionAlias;

        public ParentEntity(IParent? parent, IEntity entity, string collectionAlias)
        {
            Parent = parent;
            Entity = entity;
            _collectionAlias = collectionAlias;
        }

        public IParent? Parent { get; }
        public IEntity Entity { get; private set; }

        public ParentPath? GetParentPath()
        {
            return ParentPath.AddLevel(Parent?.GetParentPath(), _collectionAlias, Entity.Id);
        }
    }
}

namespace RapidCMS.Common.Data
{
    internal class ParentEntity : IParent
    {
        public ParentEntity(IParent? parent, IEntity entity)
        {
            Parent = parent;
            Entity = entity;
        }

        public IEntity Entity { get; private set; }

        public IParent? Parent { get; private set; }
    }
}

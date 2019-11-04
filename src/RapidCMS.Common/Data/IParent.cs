namespace RapidCMS.Common.Data
{
    public interface IParent
    {
        IParent? Parent { get; }
        IEntity Entity { get; }

        ParentPath? GetParentPath();
    }
}

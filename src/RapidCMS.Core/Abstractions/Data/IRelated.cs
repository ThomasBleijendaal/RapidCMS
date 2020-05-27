namespace RapidCMS.Core.Abstractions.Data
{
    public interface IRelated
    {
        IEntity Entity { get; }
        string CollectionAlias { get; }
    }
}

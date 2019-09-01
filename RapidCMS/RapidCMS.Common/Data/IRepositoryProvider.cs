namespace RapidCMS.Common.Data
{
    public interface IRepositoryProvider
    {
        IRepository? GetRepository(string collectionAlias);
    }
}

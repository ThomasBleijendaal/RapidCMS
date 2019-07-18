namespace RapidCMS.Common.Data
{
    public interface IQuery
    {
        int Skip { get; }
        int Take { get; }

        void HasMoreData(bool hasMoreData);

        bool MoreDataAvailable { get; }
    }
}

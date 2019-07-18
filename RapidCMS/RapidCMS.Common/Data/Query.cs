namespace RapidCMS.Common.Data
{
    public class Query : IQuery
    {
        public static IQuery Default()
        {
            return new Query
            {
                Skip = 0,
                Take = 1000
            };
        }

        public static IQuery TakeElements(int take)
        {
            return new Query
            {
                Skip = 0,
                Take = take
            };
        }

        public static IQuery Create(int pageSize, int pageNumber)
        {
            return new Query
            {
                Skip = pageSize * (pageNumber - 1),
                Take = pageSize
            };
        }

        private bool _hasMOreData = false;

        public void HasMoreData(bool hasMoreData)
        {
            _hasMOreData = hasMoreData;
        }

        public int Skip { get; set; }
        public int Take { get; set; }

        public bool MoreDataAvailable => _hasMOreData;
    }
}

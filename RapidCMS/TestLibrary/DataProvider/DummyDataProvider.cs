using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;

#nullable enable

namespace TestLibrary.DataProvider
{
    public class DummyDataProvider : IDataProvider
    {
        public Task<IEnumerable<IOption>> GetAllOptionsAsync()
        {
            IEnumerable<IOption> data = new List<Data> { new Data("a", "A"), new Data("b", "B"), new Data("3", "3"), new Data("f", "F") };
            return Task.FromResult(data);
        }

        public class Data : IOption
        {
            public Data(string id, string label)
            {
                Id = id;
                Label = label;
            }

            public string Id { get; set; }

            public string Label { get; set; }
        }
    }
}

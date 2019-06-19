using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Models;


namespace TestLibrary.DataProvider
{
    public class DummyDataProvider : IDataCollection
    {
        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            IEnumerable<IElement> data = new List<Data> {
                new Data("a", "A"),
                new Data("b", "B"),
                new Data("3", "3"),
                new Data("f", "F")
            };
            return Task.FromResult(data);
        }

        public Task SetEntityAsync(IEntity entity)
        {
            return Task.CompletedTask;
        }

        public class Data : IElement
        {
            public Data(string id, string label)
            {
                Id = id;
                Label = label;
            }

            public object Id { get; set; }

            public string Label { get; set; }
        }
    }
}

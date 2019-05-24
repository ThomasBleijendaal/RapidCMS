using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Data;

#nullable enable

namespace TestLibrary.DataProvider
{
    public class DummyDataProvider : IDataCollection
    {
        public Task AddElementAsync(IElement option)
        {
            throw new System.NotImplementedException();
        }

        //public Task<IEnumerable<IOption>> GetAllOptionsAsync()
        //{
        //    IEnumerable<IOption> data = new List<Data> { new Data("a", "A"), new Data("b", "B"), new Data("3", "3"), new Data("f", "F") };
        //    return Task.FromResult(data);
        //}

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<IElement>> GetRelatedElementsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveElementAsync(IElement option)
        {
            throw new System.NotImplementedException();
        }

        public Task SetElementAsync(IElement option)
        {
            throw new System.NotImplementedException();
        }

        //public class Data : IOption
        //{
        //    public Data(string id, string label)
        //    {
        //        Id = id;
        //        Label = label;
        //    }

        //    public object Id { get; set; }

        //    public string Label { get; set; }
        //}
    }
}

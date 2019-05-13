using Microsoft.WindowsAzure.Storage.Table;
using RapidCMS.Common.Data;

namespace TestLibrary.Entities
{
    public class AzureTableStorageEntity : TableEntity, IEntity
    {
        [IgnoreProperty]
        public string Id { get => RowKey; set => RowKey = value; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public bool? Destroy { get; set; }
    }
}

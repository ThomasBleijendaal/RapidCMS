using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using RapidCMS.Common.Interfaces;

namespace TestLibrary.Entities
{
    public class AzureTableStorageEntity : TableEntity, IEntity
    {
        [IgnoreProperty]
        public string Id { get => RowKey; set => RowKey = value; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}

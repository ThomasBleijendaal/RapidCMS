using System.Collections.Generic;
using RapidCMS.Common.Interfaces;

namespace TestLibrary.Entities
{
    public class RelationEntity : IEntity
    {
        public string Id { get => RealId.ToString(); set => RealId = int.TryParse(value, out var id) ? id : 0; }
        public int RealId { get; set; }
        public string Name { get; set; }
        public string AzureTableStorageEntityId { get; set; }
        public ICollection<string> AzureTableStorageEntityIds { get; set; }
    }
}

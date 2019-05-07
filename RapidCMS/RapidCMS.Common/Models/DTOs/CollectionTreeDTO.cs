using System.Collections.Generic;

#nullable enable

namespace RapidCMS.Common.Models.DTOs
{
    public class CollectionTreeNodeDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<CollectionTreeCollectionDTO> Collections { get; set; }
    }
    public class CollectionTreeRootDTO : CollectionTreeNodeDTO
    {

    }
    
    public class CollectionTreeCollectionDTO
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public bool RootVisible { get; set; }
        public List<CollectionTreePathDTO> Path { get; set; }
        public List<CollectionTreeNodeDTO> Nodes { get; set; }
    }

    public class CollectionTreePathDTO
    {
        public string Icon { get; set; }
        public string Path { get; set; }
    }
}

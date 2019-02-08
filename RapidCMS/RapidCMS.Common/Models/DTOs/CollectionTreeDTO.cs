using System.Collections.Generic;

namespace RapidCMS.Common.Models.DTOs
{
    public class CollectionTreeNodeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CollectionTreeCollectionDTO> Collections { get; set; }
    }
    public class CollectionTreeRootDTO : CollectionTreeNodeDTO
    {

    }

    public class CollectionTreeCollectionDTO
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public List<CollectionTreeNodeDTO> Nodes { get; set; }
    }

    public class CollectionListViewDTO
    {
        public List<CollectionListViewPaneDTO> ViewPanes { get; set; }
    }

    public class CollectionListViewPaneDTO
    {
        public Dictionary<PropertyDTO, List<string>> Properties { get; set; }
    }

    public class PropertyDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

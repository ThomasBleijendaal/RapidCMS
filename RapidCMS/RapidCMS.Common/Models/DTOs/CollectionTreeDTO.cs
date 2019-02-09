using System.Collections.Generic;
using RapidCMS.Common.Enums;

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
        public List<(PropertyDTO property, List<string> values)> Properties { get; set; }
    }

    public class PropertyDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class NodeEditorDTO
    {
        public int Id { get; set; }
        public string Alias { get; set; }

        public List<NodeEditorPaneDTO> EditorPanes { get; set; }
    }

    public class NodeEditorPaneDTO
    {
        public List<(LabelDTO label, EditorDTO editor)> Fields { get; set; }
    }

    public class LabelDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EditorDTO
    {
        public EditorType Type { get; set; }
        public object Value { get; set; }
    }
}

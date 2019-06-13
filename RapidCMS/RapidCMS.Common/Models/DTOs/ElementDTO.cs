using RapidCMS.Common.Data;


namespace RapidCMS.Common.Models.DTOs
{
    public class ElementDTO : IElement
    {
        public object Id { get; set; }
        public string Label { get; set; }
    }
}

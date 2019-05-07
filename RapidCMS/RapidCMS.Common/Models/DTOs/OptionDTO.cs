using RapidCMS.Common.Data;

#nullable enable

namespace RapidCMS.Common.Models.DTOs
{
    public class OptionDTO : IOption
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }
}

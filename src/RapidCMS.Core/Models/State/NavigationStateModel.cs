using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.State
{
    public class NavigationStateModel
    {
        public PageType PageType { get; set; }
        public UsageType UsageType { get; set; }

        public string CollectionAlias { get; set; } = default!;
        public string VariantAlias { get; set; } = default!;
        public ParentPath? ParentPath { get; set; }
        public IRelated? Related { get; set; }
        public string? Id { get; set; } = default!;

        public int? ActiveTab { get; set; } = null;
        public string? SearchTerm { get; set; } = null;
        public int CurrentPage { get; set; } = 1;
        public int? MaxPage { get; set; } = null;
    }
}

using System.Collections.Generic;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IListSetup
    {
        public int? PageSize { get; set; }
        public bool? SearchBarVisible { get; set; }
        public bool? ReorderingAllowed { get; set; }
        public ListType ListType { get; set; }
        public EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; set; }
        public List<IPaneSetup> Panes { get; set; }
        public List<IButtonSetup> Buttons { get; set; }

        public IButtonSetup? FindButton(string buttonId);
        public IEnumerable<IButtonSetup>? GetAllButtons();
    }
}

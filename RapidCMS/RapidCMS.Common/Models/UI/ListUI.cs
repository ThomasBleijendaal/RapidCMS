using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Validation;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public ListUI(EditContext rootEditContext, IEnumerable<EditContext> editContexts)
        {
            RootEditContext = rootEditContext ?? throw new ArgumentNullException(nameof(rootEditContext));
            EditContexts = editContexts ?? throw new ArgumentNullException(nameof(editContexts));
        }

        public EditContext RootEditContext { get; set; }
        public IEnumerable<EditContext> EditContexts { get; set; }

        public ListType ListType { get; internal set; }

        public List<ButtonUI>? Buttons { get; internal set; }

        public Dictionary<string, List<SectionUI>>? SectionsForEntity { get; internal set; }
    }
}

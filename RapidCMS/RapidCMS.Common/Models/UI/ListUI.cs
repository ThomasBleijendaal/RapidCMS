using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public ListUI(EditContext rootEditContext, List<EditContext> editContexts)
        {
            RootEditContext = rootEditContext ?? throw new ArgumentNullException(nameof(rootEditContext));
            EditContexts = editContexts ?? throw new ArgumentNullException(nameof(editContexts));
        }

        public EditContext RootEditContext { get; set; }
        public List<EditContext> EditContexts { get; set; }

        public ListType ListType { get; internal set; }
        public EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; internal set; }

        public List<ButtonUI>? Buttons { get; internal set; }

        public Dictionary<string, List<SectionUI>>? SectionsForEntity { get; internal set; }

        public List<FieldUI>? UniqueFields { get; internal set; }
        public List<FieldUI>? CommonFields { get; internal set; }
        public int MaxUniqueFieldsInSingleEntity { get; internal set; }
        public bool SectionsHaveButtons { get; internal set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public ListUI(
            Func<EditContext, Task<List<ButtonUI>?>> buttons, 
            Func<EditContext, Task<List<SectionUI>?>> sectionsForEntity,
            Func<EditContext, Task<List<TabUI>?>> tabs)
        {
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            SectionsForEntity = sectionsForEntity ?? throw new ArgumentNullException(nameof(sectionsForEntity));
            Tabs = tabs ?? throw new ArgumentNullException(nameof(tabs));
        }

        public ListType ListType { get; internal set; }
        public EmptyVariantColumnVisibility EmptyVariantColumnVisibility { get; internal set; }

        internal Func<EditContext, Task<List<ButtonUI>?>> Buttons { get; set; }

        internal Func<EditContext, Task<List<SectionUI>?>> SectionsForEntity { get; set; }

        internal Func<EditContext, Task<List<TabUI>?>> Tabs { get; set; }

        public List<FieldUI>? UniqueFields { get; internal set; }
        public List<FieldUI>? CommonFields { get; internal set; }
        public int MaxUniqueFieldsInSingleEntity { get; internal set; }
        public bool SectionsHaveButtons { get; internal set; }

        public int PageSize { get; internal set; }
        public bool SearchBarVisible { get; internal set; }

        // TODO: convert to real functions
        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext)
        {
            return await Buttons(editContext) ?? Enumerable.Empty<ButtonUI>();
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(EditContext editContext)
        {
            return await SectionsForEntity(editContext) ?? Enumerable.Empty<SectionUI>();
        }

        public async Task<IEnumerable<TabUI>?> GetTabsAsync(EditContext editContext)
        {
            return await Tabs(editContext);
        }
    }
}

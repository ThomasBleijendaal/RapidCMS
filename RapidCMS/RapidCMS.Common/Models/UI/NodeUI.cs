using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Models.UI
{
    // TODO: this model is a bit weird
    public class NodeUI
    {
        public NodeUI(Func<EditContext, Task<List<ButtonUI>?>> buttons, Func<EditContext, Task<List<SectionUI>?>> sections)
        {
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            Sections = sections ?? throw new ArgumentNullException(nameof(sections));
        }

        internal Func<EditContext, Task<List<ButtonUI>?>> Buttons { get; set; }
        internal Func<EditContext, Task<List<SectionUI>?>> Sections { get; set; }

        // TODO: convert to real functions
        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext)
        {
            return await Buttons(editContext) ?? Enumerable.Empty<ButtonUI>();
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(EditContext editContext)
        {
            return await Sections(editContext) ?? Enumerable.Empty<SectionUI>();
        }
    }
}

using System;
using System.Collections.Generic;
using RapidCMS.Common.Validation;

namespace RapidCMS.Common.Models.UI
{
    // TODO: move back to internal sets when weird Collection.razor NodeUI creation is no longer required
    
    public class NodeUI
    {
        public NodeUI(EditContext editContext)
        {
            EditContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
        }

        public EditContext EditContext { get; set; }

        [Obsolete("Remove me")]
        public UISubject Subject { get; }

        public List<ButtonUI> Buttons { get; set; }
        public List<SectionUI> Sections { get; set; }
    }
}

using System.Collections.Generic;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;


namespace RapidCMS.Common.Models
{
    // TODO: change Buttons to not having resolved IButtonActionHandler during config time
    internal abstract class Button
    {
        internal string ButtonId { get; set; }

        internal string Label { get; set; }
        internal string Icon { get; set; }
        internal bool ShouldConfirm { get; set; }
        internal bool IsPrimary { get; set; }

        internal List<Button> Buttons { get; set; }

        // TODO: how does this behave in custom buttons?
        internal object Metadata { get; set; }

        internal abstract CrudType GetCrudType();
        internal virtual bool IsCompatibleWithView(ViewContext viewContext) { return true; }
    }
}

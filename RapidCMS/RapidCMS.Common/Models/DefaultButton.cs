using System.Linq;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Models
{
    internal class DefaultButton : Button
    {
        internal DefaultButtonType DefaultButtonType { get; set; }

        internal override CrudType GetCrudType()
        {
            switch (DefaultButtonType)
            {
                case DefaultButtonType.New:
                    return CrudType.Create;

                case DefaultButtonType.SaveNew:
                    return CrudType.Insert;

                case DefaultButtonType.SaveExisting:
                    return CrudType.Update;

                case DefaultButtonType.Delete:
                    return CrudType.Delete;

                case DefaultButtonType.Edit:
                    return CrudType.Edit;

                case DefaultButtonType.View:
                    return CrudType.View;

                case DefaultButtonType.Remove:
                    return CrudType.Remove;

                case DefaultButtonType.Add:
                    return CrudType.Add;

                case DefaultButtonType.Pick:
                    return CrudType.Pick;

                case DefaultButtonType.Return:
                    return CrudType.Return;

                default:
                    return 0;
            }
        }
        internal override bool IsCompatibleWithForm(EditContext editContext)
        {
            return DefaultButtonType.GetCustomAttribute<ActionsAttribute>()?.Usages?.Any(x => editContext.UsageType.HasFlag(x)) ?? false;
        }
    }
}

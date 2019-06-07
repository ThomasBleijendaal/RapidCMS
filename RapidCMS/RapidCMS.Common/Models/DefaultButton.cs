using System.Linq;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;

#nullable enable

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
                    return CrudType.Read;
                case DefaultButtonType.View:
                    return CrudType.View;
                default:
                    return 0;
            }
        }
        internal override bool IsCompatibleWithView(ViewContext viewContext)
        {
            return DefaultButtonType.GetCustomAttribute<ActionsAttribute>().Usages?.Any(x => viewContext.Usage.HasFlag(x)) ?? false;
        }
    }
}

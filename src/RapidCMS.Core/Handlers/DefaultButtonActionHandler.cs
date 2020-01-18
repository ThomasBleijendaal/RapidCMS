using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Handlers
{
    internal class DefaultButtonActionHandler : IButtonActionHandler
    {
        public Task ButtonClickAfterRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, EditContext editContext, ButtonContext context)
        {
            return button.DefaultButtonType switch
            {
                DefaultButtonType.New => Task.FromResult(CrudType.Create),
                DefaultButtonType.SaveNew => Task.FromResult(CrudType.Insert),
                DefaultButtonType.SaveExisting => Task.FromResult(CrudType.Update),
                DefaultButtonType.Delete => Task.FromResult(CrudType.Delete),
                DefaultButtonType.Edit => Task.FromResult(CrudType.Edit),
                DefaultButtonType.Remove => Task.FromResult(CrudType.Remove),
                DefaultButtonType.Add => Task.FromResult(CrudType.Add),
                DefaultButtonType.Pick => Task.FromResult(CrudType.Pick),
                DefaultButtonType.Return => Task.FromResult(CrudType.Return),
                DefaultButtonType.Up => Task.FromResult(CrudType.Up),
                _ => Task.FromResult(CrudType.View),
            };
        }

        public OperationAuthorizationRequirement GetOperation(IButton button, EditContext editContext)
        {
            switch (button.DefaultButtonType)
            {
                case DefaultButtonType.New:
                case DefaultButtonType.SaveNew:
                    return Operations.Create;

                case DefaultButtonType.Edit:
                case DefaultButtonType.SaveExisting:
                    return Operations.Update;

                case DefaultButtonType.Delete:
                    return Operations.Delete;

                default:
                case DefaultButtonType.View:
                case DefaultButtonType.Return:
                case DefaultButtonType.Up:
                    return Operations.Read;

                case DefaultButtonType.Remove:
                    return Operations.Remove;

                case DefaultButtonType.Add:
                case DefaultButtonType.Pick:
                    return Operations.Add;
            }
        }

        public bool IsCompatible(IButton button, EditContext editContext)
        {
            var usages = button.DefaultButtonType.GetCustomAttribute<ActionsAttribute>()?.Usages;
            return usages?.Any(x => editContext.UsageType.HasFlag(x)) ?? false;
        }

        public bool RequiresValidForm(IButton button, EditContext editContext)
        {
            return button.DefaultButtonType.GetCustomAttribute<ValidFormAttribute>() != null;
        }

        public bool ShouldAskForConfirmation(IButton button, EditContext editContext)
        {
            return button.DefaultButtonType.GetCustomAttribute<ConfirmAttribute>() != null;
        }
    }
}

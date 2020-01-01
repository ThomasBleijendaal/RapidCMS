using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Interfaces.Handlers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Handlers
{
    internal class DefaultButtonActionHandler : IButtonActionHandler
    {
        public Task ButtonClickAfterRepositoryActionAsync(ButtonSetup button, EditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, EditContext editContext, ButtonContext context)
        {
            switch (button.DefaultButtonType)
            {
                case DefaultButtonType.New:
                    return Task.FromResult(CrudType.Create);

                case DefaultButtonType.SaveNew:
                    return Task.FromResult(CrudType.Insert);

                case DefaultButtonType.SaveExisting:
                    return Task.FromResult(CrudType.Update);

                case DefaultButtonType.Delete:
                    return Task.FromResult(CrudType.Delete);

                case DefaultButtonType.Edit:
                    return Task.FromResult(CrudType.Edit);

                default:
                case DefaultButtonType.View:
                    return Task.FromResult(CrudType.View);

                case DefaultButtonType.Remove:
                    return Task.FromResult(CrudType.Remove);

                case DefaultButtonType.Add:
                    return Task.FromResult(CrudType.Add);

                case DefaultButtonType.Pick:
                    return Task.FromResult(CrudType.Pick);

                case DefaultButtonType.Return:
                    return Task.FromResult(CrudType.Return);

                case DefaultButtonType.Up:
                    return Task.FromResult(CrudType.Up);
            }
        }

        public OperationAuthorizationRequirement GetOperation(ButtonSetup button, EditContext editContext)
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

        public bool IsCompatible(ButtonSetup button, EditContext editContext)
        {
            var usages = button.DefaultButtonType.GetCustomAttribute<ActionsAttribute>()?.Usages;
            return usages?.Any(x => editContext.UsageType.HasFlag(x)) ?? false;
        }

        public bool RequiresValidForm(ButtonSetup button, EditContext editContext)
        {
            return button.DefaultButtonType.GetCustomAttribute<ValidFormAttribute>() != null;
        }

        public bool ShouldAskForConfirmation(ButtonSetup button, EditContext editContext)
        {
            return button.DefaultButtonType.GetCustomAttribute<ConfirmAttribute>() != null;
        }
    }
}

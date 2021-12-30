﻿using System.Linq;
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
    public class DefaultButtonActionHandler : IButtonActionHandler
    {
        public Task ButtonClickAfterRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public virtual Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
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

        public OperationAuthorizationRequirement GetOperation(IButton button, FormEditContext editContext)
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

        public bool IsCompatible(IButton button, FormEditContext editContext)
        {
            var usages = button.DefaultButtonType.GetCustomAttribute<ActionsAttribute>()?.Usages;
            var isCompatible = usages?.Any(x => editContext.UsageType.HasFlag(x)) ?? false;

            return isCompatible;
        }

        public bool RequiresValidForm(IButton button, FormEditContext editContext)
        {
            return button.DefaultButtonType.GetCustomAttribute<ValidFormAttribute>() != null;
        }

        public bool ShouldAskForConfirmation(IButton button, FormEditContext editContext)
        {
            return button.DefaultButtonType.GetCustomAttribute<ConfirmAttribute>() != null;
        }
    }
}

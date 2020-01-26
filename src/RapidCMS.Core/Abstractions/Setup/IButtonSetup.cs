using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface IButtonSetup
    {
        string ButtonId { get; }
        IEnumerable<ButtonSetup> Buttons { get; }
        Type CustomType { get; }
        DefaultButtonType DefaultButtonType { get; }
        CrudType? DefaultCrudType { get; }
        EntityVariantSetup EntityVariant { get; set; }
        string Icon { get; }
        bool IsPrimary { get; }
        string Label { get; }

        Task ButtonClickAfterRepositoryActionAsync(EditContext editContext, ButtonContext context);
        Task<CrudType> ButtonClickBeforeRepositoryActionAsync(EditContext editContext, ButtonContext context);
        OperationAuthorizationRequirement GetOperation(EditContext editContext);
        bool IsCompatible(EditContext editContext);
        bool RequiresValidForm(EditContext editContext);
        bool ShouldAskForConfirmation(EditContext editContext);
    }
}

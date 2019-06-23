using System;
using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.ActionHandlers
{
    internal class DefaultButtonActionHandler : IButtonActionHandler
    {
        private readonly CrudType _crudType;
        private readonly Action? _action;

        public DefaultButtonActionHandler(CrudType crudType, Action? action)
        {
            _crudType = crudType;
            _action = action;
        }

        public CrudType GetCrudType()
        {
            return _crudType;
        }

        public Task InvokeAsync(string? parentId, string? id, object? customData)
        {
            _action?.Invoke();
            return Task.CompletedTask;
        }

        public bool IsCompatibleWithForm(EditContext editContext)
        {
            return true;
        }

        public bool RequiresValidForm()
        {
            return true;
        }

        public bool ShouldConfirm()
        {
            return false;
        }
    }
}

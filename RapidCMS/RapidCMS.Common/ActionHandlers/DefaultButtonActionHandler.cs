using System;
using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;


namespace RapidCMS.Common.ActionHandlers
{
    internal class DefaultButtonActionHandler : IButtonActionHandler
    {
        private readonly CrudType _crudType;
        private readonly Action _action;

        public DefaultButtonActionHandler(CrudType crudType, Action action)
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
            _action.Invoke();
            return Task.CompletedTask;
        }

        public bool IsCompatibleWithView(ViewContext viewContext)
        {
            return true;
        }

        public bool ShouldConfirm()
        {
            return false;
        }
    }
}

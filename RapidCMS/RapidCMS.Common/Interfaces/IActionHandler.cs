using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RapidCMS.Common.Interfaces
{
    public interface IButtonActionHandler
    {
        CrudType GetCrudType();
        bool IsCompatibleWithView(ViewContext viewContext);
        Task InvokeAsync();
    }

    public class DefaultButtonActionHandler : IButtonActionHandler
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

        public Task InvokeAsync()
        {
            _action.Invoke();
            return Task.CompletedTask;
        }

        public bool IsCompatibleWithView(ViewContext viewContext)
        {
            return true;
        }
    }
}

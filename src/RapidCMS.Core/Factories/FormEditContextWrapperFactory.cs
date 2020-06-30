using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Factories
{
    internal class FormEditContextWrapperFactory : IEditContextFactory
    {
        public IEditContext GetEditContextWrapper(EditContext editContext)
        {
            var contextType = typeof(FormEditContextWrapper<>).MakeGenericType(editContext.Entity.GetType());
            var instance = Activator.CreateInstance(contextType, editContext);

            return (IEditContext)instance;
        }

        public IEditContext GetEditContextWrapper(
            UsageType usageType,
            EntityState entityState,
            IEntity updatedEntity,
            IEntity referenceEntity,
            IParent? parent,
            IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations)
        {
            throw new NotImplementedException();
        }
    }
}

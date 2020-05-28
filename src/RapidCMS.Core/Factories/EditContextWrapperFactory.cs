using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Factories
{
    internal class EditContextWrapperFactory : IEditContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EditContextWrapperFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEditContext GetEditContextWrapper(EditContext editContext)
        {
            var contextType = typeof(FormEditContextWrapper<>).MakeGenericType(editContext.Entity.GetType());
            var instance = Activator.CreateInstance(contextType, editContext);

            return (IEditContext)instance;
        }

        public IEditContext GetEditContextWrapper(UsageType usageType, EntityState entityState, IEntity updatedEntity, IEntity referenceEntity, IParent? parent)
        {
            var contextType = typeof(ApiEditContextWrapper<>).MakeGenericType(updatedEntity.GetType());
            var instance = Activator.CreateInstance(contextType,
                usageType,
                entityState,
                updatedEntity,
                referenceEntity,
                parent,
                _serviceProvider);

            return (IEditContext)instance;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Factories
{
    // TODO: this factory has too much parameters
    internal interface IEditContextFactory
    {
        Task<IEditContext> GetEditContextWrapperAsync(FormEditContext editContext);
        Task<IEditContext> GetEditContextWrapperAsync(
            UsageType usageType, 
            EntityState entityState, 
            Type repositoryEntityType,
            IEntity updatedEntity, 
            IEntity referenceEntity, 
            IParent? parent,
            IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations);
    }
}

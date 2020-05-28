using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Factories
{
    internal interface IEditContextFactory
    {
        IEditContext GetEditContextWrapper(EditContext editContext);
        IEditContext GetEditContextWrapper(UsageType usageType, EntityState entityState, IEntity updatedEntity, IEntity referenceEntity, IParent? parent);
    }
}

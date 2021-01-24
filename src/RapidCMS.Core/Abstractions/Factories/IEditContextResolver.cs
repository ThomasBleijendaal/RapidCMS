﻿using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Factories
{
    internal interface IEditContextFactory
    {
        IEditContext GetEditContextWrapper(FormEditContext editContext);
        IEditContext GetEditContextWrapper(
            UsageType usageType, 
            EntityState entityState, 
            Type repositoryEntityType,
            IEntity updatedEntity, 
            IEntity referenceEntity, 
            IParent? parent,
            IEnumerable<(string propertyName, string typeName, IEnumerable<object> elements)> relations);
    }
}

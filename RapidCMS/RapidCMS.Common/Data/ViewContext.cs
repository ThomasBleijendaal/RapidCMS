using System;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;

#nullable enable

namespace RapidCMS.Common.Data
{
    public sealed class ViewContext
    {
        public ViewContext(UsageType usage, EntityVariant entityVariant)
        {
            Usage = usage;
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
        }

        public UsageType Usage { get; private set; }
        public EntityVariant EntityVariant { get; private set; }
    }
}

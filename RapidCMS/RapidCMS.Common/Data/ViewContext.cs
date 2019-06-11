using System;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;


namespace RapidCMS.Common.Data
{
    public sealed class ViewContext
    {
        public ViewContext(UsageType usage, EntityVariant entityVariant, IEntity representativeEntity)
        {
            Usage = usage;
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
            RepresentativeEntity = representativeEntity ?? throw new ArgumentNullException(nameof(representativeEntity));
        }

        public UsageType Usage { get; private set; }
        public EntityVariant EntityVariant { get; private set; }
        public IEntity RepresentativeEntity { get; private set; }
    }
}

using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Request.Api
{
    public class PersistEntityRequestModel
    {
        public IEntity Entity { get; set; } = default!;
        public EntityDescriptor Descriptor { get; set; } = default!;
        public EntityState EntityState { get; set; }
    }
}

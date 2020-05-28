using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class OrderModel
    {
        public string PropertyName { get; set; } = default!;
        public string Fingerprint { get; set; } = default!;
        public OrderByType OrderByType { get; set; }
    }
}

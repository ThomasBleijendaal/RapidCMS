namespace RapidCMS.Core.Models.Request.Api
{
    public class PersistReorderRequestModel
    {
        public string? BeforeId { get; set; }
        public EntityDescriptor Subject { get; set; } = default!;
    }
}

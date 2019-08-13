namespace RapidCMS.Common.Forms
{
    public sealed class ButtonContext
    {
        public ButtonContext(string? parentId, object? customData)
        {
            ParentId = parentId;
            CustomData = customData;
        }

        public string? ParentId { get; set; }
        public object? CustomData { get; set; }
    }
}

using RapidCMS.Common.Data;

namespace RapidCMS.Common.Forms
{
    public sealed class ButtonContext
    {
        public ButtonContext(IParent? parent, object? customData)
        {
            Parent = parent;
            CustomData = customData;
        }

        public IParent? Parent { get; set; }
        public object? CustomData { get; set; }
    }
}

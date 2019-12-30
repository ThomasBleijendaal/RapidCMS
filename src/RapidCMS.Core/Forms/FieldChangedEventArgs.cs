using RapidCMS.Core.Interfaces.Metadata;

namespace RapidCMS.Core.Forms
{
    public class FieldChangedEventArgs
    {
        public FieldChangedEventArgs(IPropertyMetadata fullPropertyMetadata)
        {
            FullPropertyMetadata = fullPropertyMetadata;
        }

        public IPropertyMetadata FullPropertyMetadata { get; }
    }
}

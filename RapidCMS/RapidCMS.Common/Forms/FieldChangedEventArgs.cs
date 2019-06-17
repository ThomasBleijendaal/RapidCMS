using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Forms
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

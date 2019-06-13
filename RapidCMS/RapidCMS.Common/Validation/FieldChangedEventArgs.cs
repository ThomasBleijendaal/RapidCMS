using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Validation
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

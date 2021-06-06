using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ITreeElementSetup
    {
        string Alias { get; }
        string Name { get; }
        PageType Type { get; }
        CollectionRootVisibility RootVisibility { get; }
    }
}

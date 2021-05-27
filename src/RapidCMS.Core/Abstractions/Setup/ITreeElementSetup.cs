using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ITreeElementSetup
    {
        string Alias { get; }
        string Name { get; }
        PageType Type { get; }
        CollectionRootVisibility RootVisibility { get; }
    }
}

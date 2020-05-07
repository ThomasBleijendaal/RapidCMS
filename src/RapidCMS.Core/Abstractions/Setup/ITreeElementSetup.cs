using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ITreeElementSetup
    {
        string Alias { get; }
        PageType Type { get; }

        CollectionRootVisibility RootVisibility { get; }
        bool DefaultOpenCollections { get; }
    }
}

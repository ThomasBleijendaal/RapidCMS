using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface IPageSetup : ITreeElementSetup
    {
        string Icon { get; }
        string Name { get; }
        List<ITypeRegistration> Sections { get; }
    }
}

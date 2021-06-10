using System.Collections.Generic;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions
{
    public interface IInformation
    {
        bool IsValid();

        IEnumerable<string> NamespacesUsed();
    }
}

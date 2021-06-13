using System.Collections.Generic;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions
{
    internal interface IInformation
    {
        bool IsValid();

        IEnumerable<string> NamespacesUsed(Use use);
    }
}

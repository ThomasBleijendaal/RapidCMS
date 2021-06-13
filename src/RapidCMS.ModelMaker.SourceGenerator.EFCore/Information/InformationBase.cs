using System.Collections.Generic;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal abstract class InformationBase
    {
        protected readonly List<(Use use, string @namespace)> _namespaces = new();
    }
}

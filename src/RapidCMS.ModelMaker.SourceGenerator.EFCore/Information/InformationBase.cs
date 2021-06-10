using System.Collections.Generic;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal abstract class InformationBase
    {
        protected readonly List<string> _namespaces = new();
    }
}

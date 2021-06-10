using System.Collections.Generic;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal abstract class InformationBase
    {
        // TODO: filter these by use (Entity, Collection, Repository)
        protected readonly List<string> _namespaces = new();
    }
}

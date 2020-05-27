using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class ParentQueryModel : QueryModel
    {
        public ParentQueryModel() { }

        public ParentQueryModel(IParent? parent)
        {
            ParentPath = parent?.GetParentPath()?.ToPathString();
        }

        public ParentQueryModel(IParent? parent, IQuery query) : this(parent)
        {
            Skip = query.Skip;
            Take = query.Take;
            SearchTerm = query.SearchTerm;
        }

        public ParentQueryModel(IParent? parent, Type? variantType) : this(parent)
        {
            VariantTypeName = variantType?.AssemblyQualifiedName;
        }

        public ParentQueryModel(IParent? parent, Type? variantType, IQuery query) : this(parent, query)
        {
            VariantTypeName = variantType?.AssemblyQualifiedName;
        }

        public string? ParentPath { get; set; }
    }
}

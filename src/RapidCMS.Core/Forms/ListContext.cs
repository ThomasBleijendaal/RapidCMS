using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Forms
{
    public sealed class ListContext
    {
        public ListContext(string collectionAlias, FormEditContext protoEditContext, IParent? parent, UsageType usageType, List<FormEditContext>? editContexts, IServiceProvider serviceProvider)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
            ProtoEditContext = protoEditContext ?? throw new ArgumentNullException(nameof(protoEditContext));
            Parent = parent;
            UsageType = usageType;
            EditContexts = editContexts ?? new List<FormEditContext>();
            ServiceProvider = serviceProvider;
        }

        public string CollectionAlias { get; private set; }
        public FormEditContext ProtoEditContext { get; private set; }
        public IParent? Parent { get; private set; }
        public UsageType UsageType { get; private set; }
        public List<FormEditContext> EditContexts { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }
    }
}

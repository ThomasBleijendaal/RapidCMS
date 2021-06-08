using System;
using System.Collections.Generic;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IPaneSetup
    {
        public Type? CustomType { get; set; }
        public string? Label { get; set; }
        public Func<object, EntityState, bool> IsVisible { get; set; }
        public Type VariantType { get; set; }
        public List<IButtonSetup> Buttons { get; set; }
        public List<IFieldSetup> Fields { get; set; }
        public List<ISubCollectionListSetup> SubCollectionLists { get; set; }
        public List<IRelatedCollectionListSetup> RelatedCollectionLists { get; set; }
    }
}

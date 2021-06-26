using System.Collections.Generic;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.EqualityComparers
{
    internal class PropertyValidationModelEqualityComparer : IEqualityComparer<PropertyDetailModel>
    {
        public bool Equals(PropertyDetailModel? x, PropertyDetailModel? y) => x?.Alias == y?.Alias;

        public int GetHashCode(PropertyDetailModel obj) => obj.Alias.GetHashCode();
    }
}

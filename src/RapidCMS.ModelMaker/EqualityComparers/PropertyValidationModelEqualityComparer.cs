using System.Collections.Generic;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.EqualityComparers
{
    internal class PropertyValidationModelEqualityComparer : IEqualityComparer<PropertyValidationModel>
    {
        public bool Equals(PropertyValidationModel? x, PropertyValidationModel? y) => x?.Alias == y?.Alias;

        public int GetHashCode(PropertyValidationModel obj) => obj.Alias.GetHashCode();
    }
}

using System.Linq;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Extenstions
{
    public static class PropertyModelExtensions
    {
        public static PropertyDetailModel GetValidation(this PropertyModel property, string alias)
            => property.Details.First(x => x.Alias == alias);

        public static PropertyDetailModel? TryGetValidation(this PropertyModel property, string alias)
            => property.Details?.FirstOrDefault(x => x.Alias == alias);
    }
}

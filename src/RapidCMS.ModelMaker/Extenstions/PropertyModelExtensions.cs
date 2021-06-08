using System.Linq;
using RapidCMS.ModelMaker.Models.Entities;

namespace RapidCMS.ModelMaker.Extenstions
{
    public static class PropertyModelExtensions
    {
        public static PropertyValidationModel GetValidation(this PropertyModel property, string alias)
            => property.Validations.First(x => x.Alias == alias);

        public static PropertyValidationModel? TryGetValidation(this PropertyModel property, string alias)
            => property.Validations?.FirstOrDefault(x => x.Alias == alias);
    }
}

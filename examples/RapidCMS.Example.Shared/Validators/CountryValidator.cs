using FluentValidation;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Validators
{

    public class CountryValidator : AbstractValidatorAdapter<Country>
    {
        public CountryValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotEqual("fdsa");
            RuleFor(x => x.Metadata.Continent).NotEmpty().MinimumLength(8).MaximumLength(10).NotEqual("fdsafdsa");
            RuleFor(x => x.People).Must(x => x.Count <= 2);
        }
    }
}

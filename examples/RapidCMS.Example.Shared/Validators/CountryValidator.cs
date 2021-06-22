using System.Collections.Generic;
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
        }
    }

    public class CountryRelationValidator : AbstractRelationValidatorAdapter<Country>
    {
        public CountryRelationValidator()
        {
            RuleFor(x => x.GetRelatedElementIdsFor<Country, IEnumerable<Person>, int>(x => x.People)).Must(x => x?.Count <= 2)
                .WithName("People")
                .WithMessage("Only two items can be selected.");
        }
    }
}

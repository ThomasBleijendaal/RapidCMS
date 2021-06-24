using System.Collections.Generic;
using FluentValidation;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Validators
{
    public class CountryValidator : AbstractValidatorAdapter<Country>
    {
        public CountryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Must(x => x != (_context.Configuration as Config)?.ForbiddenCountryName)
                    .WithMessage(x => $"The name cannot be '{(_context.Configuration as Config)?.ForbiddenCountryName}'.");
            RuleFor(x => x.Metadata.Continent)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(10)
                .Must(x => x != (_context.Configuration as Config)?.ForbiddenContinentName)
                    .WithMessage(x => $"The continent cannot be '{(_context.Configuration as Config)?.ForbiddenContinentName}'.");
            ;
            RuleFor(x => x.People)
                .Must(original =>
                {
                    var selectedPeople = _context.RelationContainer.GetRelatedElementIdsFor<Country, IEnumerable<Person>, int>(x => x.People);

                    return selectedPeople?.Count <= 2;
                })
                    .WithMessage("Only two items can be selected.");
        }

        public class Config
        {
            public string? ForbiddenCountryName { get; set; }
            public string? ForbiddenContinentName { get; set; }
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Validators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Core.Tests.Forms;

public class EditContextCustomValidationConfigurationTests
{
    private FormEditContext _subject = default!;
    private ServiceCollection _serviceCollection = new ServiceCollection();

    [Test]
    public async Task WhenInvalidEntityIsValidated_ThenIsValidShouldReturnFalseAsync()
    {
        _serviceCollection.AddSingleton<ConfigurableEntityValidator>();

        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { Id = "abc" },
            default,
            UsageType.Edit,
            new List<ValidationSetup>
            {
                new ValidationSetup(typeof(ConfigurableEntityValidator), new ConfigurableEntityValidator.Config { InvalidId = "abc" })
            },
            _serviceCollection.BuildServiceProvider());

        _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

        // assert
        Assert.That(await _subject.IsValidAsync(), Is.False);
        Assert.That(await _subject.IsValidAsync(), Is.False);
        Assert.AreEqual("Id is null", _subject.GetPropertyState("Id").GetValidationMessages().First());
    }

    [Test]
    public async Task WhenValidEntityIsValidated_ThenIsValidShouldReturnTrueAsync()
    {
        _serviceCollection.AddSingleton<ConfigurableEntityValidator>();

        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity
            {
                Id = "abcd"
            },
            default,
            UsageType.Edit,
            new List<ValidationSetup>
            {
                new ValidationSetup(typeof(ConfigurableEntityValidator), new ConfigurableEntityValidator.Config { InvalidId = "abc" })
            },
            _serviceCollection.BuildServiceProvider());

        _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

        // assert
        Assert.IsTrue(await _subject.IsValidAsync());
    }

    public class Entity : IEntity
    {
        public string Id { get; set; }

        public int X { get; set; }
    }

    public class ConfigurableEntityValidator : BaseEntityValidator<Entity>
    {
        public override IEnumerable<ValidationResult> Validate(IValidatorContext<Entity> context)
        {
            if (!string.IsNullOrEmpty(context.Entity.Id) && context.Entity.Id != (context.Configuration as Config)?.InvalidId)
            {
                yield break;
            }
            else
            {
                yield return new ValidationResult("Id is null", new[] { nameof(context.Entity.Id) });
            }
        }

        public class Config
        {
            public string InvalidId { get; set; }
        }
    }
}

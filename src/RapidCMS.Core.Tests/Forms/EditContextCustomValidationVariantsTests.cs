using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Setup;
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

namespace RapidCMS.Core.Tests.Forms
{
    public class EditContextCustomValidationVariantsTests
    {
        private FormEditContext _subject = default!;
        private ServiceCollection _serviceCollection = new ServiceCollection();

        [Test]
        public async Task WhenInvalidEntityIsValidated_ThenIsValidShouldReturnFalseAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();
            _serviceCollection.AddSingleton<EntityVariantAValidator>();
            _serviceCollection.AddSingleton<EntityVariantBValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity(),
                default,
                UsageType.Edit,
                new List<IValidationSetup>
                {
                    new ValidationSetup(typeof(EntityValidator), default),
                    new ValidationSetup(typeof(EntityVariantAValidator), default),
                    new ValidationSetup(typeof(EntityVariantBValidator), default)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsFalse(await _subject.IsValidAsync());
            Assert.AreEqual("Id is null", _subject.GetPropertyState("Id").GetValidationMessages().First());
        }

        [Test]
        public async Task WhenInvalidEntityVariantAIsValidated_ThenIsValidShouldReturnFalseAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();
            _serviceCollection.AddSingleton<EntityVariantAValidator>();
            _serviceCollection.AddSingleton<EntityVariantBValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new EntityVariantA { Id = "A" },
                default,
                UsageType.Edit,
                new List<IValidationSetup>
                {
                    new ValidationSetup(typeof(EntityValidator), default),
                    new ValidationSetup(typeof(EntityVariantAValidator), default),
                    new ValidationSetup(typeof(EntityVariantBValidator), default)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsFalse(await _subject.IsValidAsync());
            Assert.AreEqual("Id of A-variant cannot be A", _subject.GetPropertyState("Id").GetValidationMessages().First());
        }

        [Test]
        public async Task WhenInvalidEntityVariantBIsValidated_ThenIsValidShouldReturnFalseAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();
            _serviceCollection.AddSingleton<EntityVariantAValidator>();
            _serviceCollection.AddSingleton<EntityVariantBValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new EntityVariantB { Id = "B" },
                default,
                UsageType.Edit,
                new List<IValidationSetup>
                {
                    new ValidationSetup(typeof(EntityValidator), default),
                    new ValidationSetup(typeof(EntityVariantAValidator), default),
                    new ValidationSetup(typeof(EntityVariantBValidator), default)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsFalse(await _subject.IsValidAsync());
            Assert.AreEqual("Id of B-variant cannot be B", _subject.GetPropertyState("Id").GetValidationMessages().First());
        }

        [Test]
        public async Task WhenValidEntityIsValidated_ThenIsValidShouldReturnTrueAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();
            _serviceCollection.AddSingleton<EntityVariantAValidator>();
            _serviceCollection.AddSingleton<EntityVariantBValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity
                {
                    Id = "valid"
                },
                default,
                UsageType.Edit,
                new List<IValidationSetup>
                {
                    new ValidationSetup(typeof(EntityValidator), default),
                    new ValidationSetup(typeof(EntityVariantAValidator), default),
                    new ValidationSetup(typeof(EntityVariantBValidator), default)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsTrue(await _subject.IsValidAsync());
        }

        [Test]
        public async Task WhenValidEntityVariantAIsValidated_ThenIsValidShouldReturnTrueAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();
            _serviceCollection.AddSingleton<EntityVariantAValidator>();
            _serviceCollection.AddSingleton<EntityVariantBValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new EntityVariantA { Id = "B" },
                default,
                UsageType.Edit,
                new List<IValidationSetup>
                {
                    new ValidationSetup(typeof(EntityValidator), default),
                    new ValidationSetup(typeof(EntityVariantAValidator), default),
                    new ValidationSetup(typeof(EntityVariantBValidator), default)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsTrue(await _subject.IsValidAsync());
        }

        [Test]
        public async Task WhenValidEntityVariantBIsValidated_ThenIsValidShouldReturnTrueAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();
            _serviceCollection.AddSingleton<EntityVariantAValidator>();
            _serviceCollection.AddSingleton<EntityVariantBValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new EntityVariantB { Id = "A" },
                default,
                UsageType.Edit,
                new List<IValidationSetup>
                {
                    new ValidationSetup(typeof(EntityValidator), default),
                    new ValidationSetup(typeof(EntityVariantAValidator), default),
                    new ValidationSetup(typeof(EntityVariantBValidator), default)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsTrue(await _subject.IsValidAsync());
        }

        public class Entity : IEntity
        {
            public string Id { get; set; }
        }

        public class EntityVariantA : Entity
        {

        }

        public class EntityVariantB : Entity
        {

        }

        public class EntityValidator : BaseEntityValidator<Entity>
        {
            public override IEnumerable<ValidationResult> Validate(IValidatorContext<Entity> context)
            {
                if (!string.IsNullOrEmpty(context.Entity.Id))
                {
                    yield break;
                }
                else
                {
                    yield return new ValidationResult("Id is null", new[] { nameof(context.Entity.Id) });
                }
            }
        }

        public class EntityVariantAValidator : BaseEntityValidator<EntityVariantA>
        {
            public override IEnumerable<ValidationResult> Validate(IValidatorContext<EntityVariantA> context)
            {
                if (context.Entity.Id != "A")
                {
                    yield break;
                }
                else
                {
                    yield return new ValidationResult("Id of A-variant cannot be A", new[] { nameof(context.Entity.Id) });
                }
            }
        }

        public class EntityVariantBValidator : BaseEntityValidator<EntityVariantB>
        {
            public override IEnumerable<ValidationResult> Validate(IValidatorContext<EntityVariantB> context)
            {
                if (context.Entity.Id != "B")
                {
                    yield break;
                }
                else
                {
                    yield return new ValidationResult("Id of B-variant cannot be B", new[] { nameof(context.Entity.Id) });
                }
            }
        }
    }
}

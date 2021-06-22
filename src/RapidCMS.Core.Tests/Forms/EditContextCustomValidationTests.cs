using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RapidCMS.Core.Tests.Forms
{
    public class EditContextCustomValidationTests
    {
        private FormEditContext _subject = default!;
        private ServiceCollection _serviceCollection = new ServiceCollection();

        [Test]
        public async Task WhenInvalidEntityIsValidated_ThenIsValidShouldReturnFalseAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();

            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity(),
                default,
                UsageType.Edit,
                new List<Type>
                {
                    typeof(EntityValidator)
                },
                _serviceCollection.BuildServiceProvider());

            _subject.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata<Entity, string>(x => x.Id));

            // assert
            Assert.IsFalse(await _subject.IsValidAsync());
            Assert.AreEqual("Id is null", _subject.GetPropertyState("Id").GetValidationMessages().First());
        }

        [Test]
        public async Task WhenValidEntityIsValidated_ThenIsValidShouldReturnFalseAsync()
        {
            _serviceCollection.AddSingleton<EntityValidator>();

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
                new List<Type>
                {
                    typeof(EntityValidator)
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

        public class EntityValidator : BaseEntityValidator<Entity>
        {
            protected override IEnumerable<ValidationResult> Validate(Entity entity)
            {
                if (!string.IsNullOrEmpty(entity.Id))
                {
                    yield break;
                }
                else
                {
                    yield return new ValidationResult("Id is null", new[] { nameof(entity.Id) });
                }
            }
        }
    }
}

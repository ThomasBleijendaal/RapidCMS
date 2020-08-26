using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Forms.Validation;
using RapidCMS.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace RapidCMS.Core.Tests.Forms
{
    public class EditContextTests
    {
        private FormEditContext _subject = default!;
        private ServiceCollection _serviceCollection = new ServiceCollection();

        [SetUp]
        public void Setup()
        {
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { Id = "1" },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());
        }

        [Test]
        public void WhenPropertyIsModifiedViaMetadata_ThenFieldEventIsTriggered()
        {
            // arrange
            var called = false;
            _subject.OnFieldChanged += (o, e) => called = true;

            // act
            _subject.NotifyPropertyChanged(_property);

            // assert
            Assert.IsTrue(called);
        }

        [Test]
        public void WhenPropertyIsBusyViaMetadata_ThenValidationStateEventIsTriggered()
        {
            // arrange
            var called = false;
            _subject.OnValidationStateChanged += (o, e) => called = true;

            // act
            _subject.NotifyPropertyBusy(_property);

            // assert
            Assert.IsTrue(called);
        }

        [Test]
        public void WhenPropertyIsFinishedViaMetadata_ThenValidationStateEventIsTriggered()
        {
            // arrange
            var called = false;
            _subject.OnValidationStateChanged += (o, e) => called = true;

            // act
            _subject.NotifyPropertyFinished(_property);

            // assert
            Assert.IsTrue(called);
        }

        [Test]
        public void WhenEntityIsInvalidButNotTouched_ThenEditContextIsValid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());

            // act & assert
            Assert.IsTrue(_subject.IsValid());
        }

        [Test]
        public void WhenEntityIsInvalidButTouched_ThenEditContextIsInvalid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());
            _subject.NotifyPropertyIncludedInForm(_property);

            // act & assert
            Assert.IsFalse(_subject.IsValid());
        }

        [Test]
        public void WhenEntityIsValidAndTouched_ThenEditContextIsValid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { Id = "123" },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());
            _subject.NotifyPropertyChanged(_property);

            // act & assert
            Assert.IsTrue(_subject.IsValid());
        }

        [Test]
        public void WhenPropertyIsInvalidButNotTouched_ThenEditContextIsValid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());

            // act & assert
            Assert.IsTrue(_subject.IsValid(_property));
        }

        [Test]
        public void WhenPropertyIsInvalidButTouched_ThenEditContextIsInvalid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());
            _subject.NotifyPropertyChanged(_property);

            // act & assert
            Assert.IsFalse(_subject.IsValid(_property));
        }

        [Test]
        public void WhenPropertyIsValidAndTouched_ThenEditContextIsValid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { Id = "123" },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());
            _subject.NotifyPropertyIncludedInForm(_property);

            // act & assert
            Assert.IsTrue(_subject.IsValid(_property));
        }

        [Test]
        public void WhenEntityIsNotTouched_ThenEditContextIsNotModified()
        {
            // act & assert
            Assert.IsFalse(_subject.IsModified());
        }

        [Test]
        public void WhenEntityIsTouched_ThenEditContextIsModified()
        {
            // arrange
            _subject.NotifyPropertyChanged(_property);

            // act & assert
            Assert.IsTrue(_subject.IsModified());
        }

        [Test]
        public void WhenPropertyIsNotTouched_ThenPropertyIsNotValidated()
        {
            // act & assert
            Assert.IsFalse(_subject.WasValidated(_property));
        }

        [Test]
        public void WhenPropertyIsTouched_ThenPropertyIsValidated()
        {
            // arrange
            _subject.NotifyPropertyChanged(_property);

            // act & assert
            Assert.IsTrue(_subject.WasValidated(_property));
        }

        [Test]
        public void WhenValidationMessageIsAddedToProperty_ThenPropertyIsInvalid()
        {
            // act
            _subject.AddValidationMessage(_property, "Error");

            // assert
            Assert.IsTrue(_subject.WasValidated(_property));
            Assert.IsFalse(_subject.IsValid(_property));
        }

        [Test]
        public void WhenValidationMessageIsAddedToProperty_ThenPropertyHasValidationMessage()
        {
            // act
            _subject.AddValidationMessage(_property, "Error");

            // assert
            Assert.AreEqual("Error", _subject.GetValidationMessages(_property).First());
        }

        [Test]
        public void WhenNestedPropertyIsTouched_ThenPropertyIsValidated()
        {
            // act
            _subject.NotifyPropertyChanged(_nestedProperty);

            // assert
            Assert.IsTrue(_subject.WasValidated(_nestedProperty));
            Assert.IsFalse(_subject.IsValid(_nestedProperty));
        }

        [Test]
        public void WhenNestedPropertyIsTouchedAndValid_ThenPropertyIsValidatedAndValid()
        {
            // arrange
            _subject = new FormEditContext(
                "alias",
                "repoAlias",
                "variantAlias",
                new Entity { Id = "123", Nested = new Entity.NestedObject { Data = "456" } },
                default,
                UsageType.Edit,
                _serviceCollection.BuildServiceProvider());

            // act
            _subject.NotifyPropertyChanged(_nestedProperty);

            // assert
            Assert.IsTrue(_subject.WasValidated(_nestedProperty));
            Assert.IsTrue(_subject.IsValid(_nestedProperty));
        }

        [Test]
        public void WhenNestedPropertyIsNotTouched_ThenPropertyIsNotValidated()
        {
            // assert
            Assert.IsFalse(_subject.WasValidated(_nestedProperty));
            Assert.IsTrue(_subject.IsValid(_nestedProperty));
        }

        private readonly Expression<Func<Entity, string?>> _getId = x => x.Id;
        private readonly Expression<Func<Entity, string?>> _getNestedData = x => x.Nested.Data;
        private IPropertyMetadata _property => PropertyMetadataHelper.GetPropertyMetadata(_getId)!;
        private IPropertyMetadata _nestedProperty => PropertyMetadataHelper.GetPropertyMetadata(_getNestedData)!;
        public class Entity : IEntity
        {
            [Required]
            public string? Id { get; set; }

            [ValidateObject]
            public NestedObject Nested { get; set; } = new NestedObject();

            public class NestedObject
            {
                [Required]
                public string? Data { get; set; }
            }
        }
    }
}

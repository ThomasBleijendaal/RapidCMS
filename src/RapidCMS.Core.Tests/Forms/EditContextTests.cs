using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Tests.Forms
{
    // TODO: expand tests with CompositeValidationResult 
    public class EditContextTests
    {
        private EditContext _subject = default!;
        private ServiceCollection _serviceCollection = new ServiceCollection();

        [SetUp]
        public void Setup()
        {
            _subject = new EditContext(
                "alias",
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
            _subject = new EditContext(
                "alias",
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
            _subject = new EditContext(
                "alias",
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
            _subject = new EditContext(
                "alias",
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
            _subject = new EditContext(
                "alias",
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
            _subject = new EditContext(
                "alias",
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
            _subject = new EditContext(
                "alias",
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
            Assert.IsFalse(_subject.WasValidated(_property));
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

        private readonly Expression<Func<Entity, string?>> _getId = x => x.Id;
        private IPropertyMetadata _property => PropertyMetadataHelper.GetPropertyMetadata(_getId)!;
        public class Entity : IEntity
        {
            [Required]
            public string? Id { get; set; }
        }
    }
}

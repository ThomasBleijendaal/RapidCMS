using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Attributes;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RapidCMS.Core.Tests.Forms;

public class EditContextTests
{
    private FormEditContext _subject = default!;
    private ServiceCollection _serviceCollection = new ServiceCollection();

    [SetUp]
    public void Setup()
    {
        _serviceCollection.AddSingleton<DataAnnotationEntityValidator>();

        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { Id = "1" },
            default,
            UsageType.Edit,
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());
    }

    [Test]
    public async Task WhenPropertyIsModifiedViaMetadata_ThenFieldEventIsTriggeredAsync()
    {
        // arrange
        var called = false;
        _subject.OnFieldChanged += (o, e) => called = true;

        // act
        await _subject.NotifyPropertyChangedAsync(_property);

        // assert
        Assert.That(called, Is.True);
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
        Assert.That(called, Is.True);
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
        Assert.That(called, Is.True);
    }

    [Test]
    public async Task WhenEntityIsInvalidButNotTouched_ThenEditContextIsValidAsync()
    {
        // arrange
        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { },
            default,
            UsageType.Edit,
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());

        // act & assert
        Assert.That(await _subject.IsValidAsync(), Is.True);
    }

    [Test]
    public async Task WhenEntityIsInvalidButTouched_ThenEditContextIsInvalidAsync()
    {
        // arrange
        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { },
            default,
            UsageType.Edit,
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());
        _subject.NotifyPropertyIncludedInForm(_property);

        // act & assert
        Assert.That(await _subject.IsValidAsync(), Is.False);
    }

    [Test]
    public async Task WhenEntityIsValidAndTouched_ThenEditContextIsValidAsync()
    {
        // arrange
        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { Id = "123" },
            default,
            UsageType.Edit,
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());

        await _subject.NotifyPropertyChangedAsync(_property);

        // act & assert
        Assert.That(await _subject.IsValidAsync(), Is.True);
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
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());

        // act & assert
        Assert.That(_subject.IsValid(_property), Is.True);
    }

    [Test]
    public async Task WhenPropertyIsInvalidButTouched_ThenEditContextIsInvalidAsync()
    {
        // arrange
        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { },
            default,
            UsageType.Edit,
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());

        await _subject.NotifyPropertyChangedAsync(_property);

        // act & assert
        Assert.That(_subject.IsValid(_property), Is.False);
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
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());
        _subject.NotifyPropertyIncludedInForm(_property);

        // act & assert
        Assert.That(_subject.IsValid(_property), Is.True);
    }

    [Test]
    public void WhenEntityIsNotTouched_ThenEditContextIsNotModified()
    {
        // act & assert
        Assert.That(_subject.IsModified(), Is.False);
    }

    [Test]
    public async Task WhenEntityIsTouched_ThenEditContextIsModifiedAsync()
    {
        // arrange
        await _subject.NotifyPropertyChangedAsync(_property);

        // act & assert
        Assert.That(_subject.IsModified(), Is.True);
    }

    [Test]
    public void WhenPropertyIsNotTouched_ThenPropertyIsNotValidated()
    {
        // act & assert
        Assert.That(_subject.WasValidated(_property), Is.False);
    }

    [Test]
    public async Task WhenPropertyIsTouched_ThenPropertyIsValidatedAsync()
    {
        // arrange
        await _subject.NotifyPropertyChangedAsync(_property);

        // act & assert
        Assert.That(_subject.WasValidated(_property), Is.True);
    }

    [Test]
    public void WhenValidationMessageIsAddedToProperty_ThenPropertyIsInvalid()
    {
        // act
        _subject.AddValidationMessage(_property, "Error");

        // assert
        Assert.That(_subject.WasValidated(_property), Is.True);
        Assert.That(_subject.IsValid(_property), Is.False);
    }

    [Test]
    public void WhenValidationMessageIsAddedToProperty_ThenPropertyHasValidationMessage()
    {
        // act
        _subject.AddValidationMessage(_property, "Error");

        // assert
        Assert.That(_subject.GetValidationMessages(_property).First(), Is.EqualTo("Error"));
    }

    [Test]
    public async Task WhenNestedPropertyIsTouched_ThenPropertyIsValidatedAsync()
    {
        // act
        await _subject.NotifyPropertyChangedAsync(_nestedProperty);

        // assert
        Assert.That(_subject.WasValidated(_nestedProperty), Is.True);
        Assert.That(_subject.IsValid(_nestedProperty), Is.False);
    }

    [Test]
    public async Task WhenNestedPropertyIsTouchedAndValid_ThenPropertyIsValidatedAndValidAsync()
    {
        // arrange
        _subject = new FormEditContext(
            "alias",
            "repoAlias",
            "variantAlias",
            new Entity { Id = "123", Nested = new Entity.NestedObject { Data = "456" } },
            default,
            UsageType.Edit,
            new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) },
            _serviceCollection.BuildServiceProvider());

        // act
        await _subject.NotifyPropertyChangedAsync(_nestedProperty);

        // assert
        Assert.That(_subject.WasValidated(_nestedProperty), Is.True);
        Assert.That(_subject.IsValid(_nestedProperty), Is.True);
    }

    [Test]
    public void WhenNestedPropertyIsNotTouched_ThenPropertyIsNotValidated()
    {
        // assert
        Assert.That(_subject.WasValidated(_nestedProperty), Is.False);
        Assert.That(_subject.IsValid(_nestedProperty), Is.True);
    }

    private readonly Expression<Func<Entity, string>> _getId = x => x.Id;
    private readonly Expression<Func<Entity, string>> _getNestedData = x => x.Nested.Data;
    private IPropertyMetadata _property => PropertyMetadataHelper.GetPropertyMetadata(_getId)!;
    private IPropertyMetadata _nestedProperty => PropertyMetadataHelper.GetPropertyMetadata(_getNestedData)!;
    public class Entity : IEntity
    {
        [Required]
        public string Id { get; set; }

        [ValidateObject]
        public NestedObject Nested { get; set; } = new NestedObject();

        public class NestedObject
        {
            [Required]
            public string Data { get; set; }
        }
    }
}

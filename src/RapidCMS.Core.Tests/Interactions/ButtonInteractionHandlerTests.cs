using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Interactions;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Tests.Services.Dispatchers;
using RapidCMS.Core.Validators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RapidCMS.Core.Tests.Interactions
{
    public class ButtonInteractionHandlerTests
    {
        private IButtonInteraction _subject = default!;

        private Mock<ISetupResolver<CollectionSetup>> _collectionResolver = default!;
        private Mock<IButtonActionHandler> _buttonActionHandler = default!;
        private Mock<IButtonActionHandlerResolver> _buttonActionHandlerResolver = default!;
        private Mock<IAuthService> _authService = default!;
        private ButtonSetup _button = default!;
        private CollectionSetup _collection = default!;
        private IServiceProvider _serviceProvider = default!;

        [SetUp]
        public void Setup()
        {
            _button = new ButtonSetup
            {
                ButtonId = "123",
                Buttons = new List<ButtonSetup>()
            };

            _collection = new CollectionSetup("icon", "color", "name", "alias", "repo")
            {
                ListEditor = new ListSetup(
                    null,
                    null,
                    null,
                    ListType.Table,
                    EmptyVariantColumnVisibility.Visible,
                    new List<PaneSetup>(),
                    new List<ButtonSetup>
                    {
                        _button
                    })
            };

            _collectionResolver = new Mock<ISetupResolver<CollectionSetup>>();
            _collectionResolver
                .Setup(x => x.ResolveSetupAsync(It.IsAny<string>()))
                .ReturnsAsync(_collection);

            _buttonActionHandler = new Mock<IButtonActionHandler>();
            _buttonActionHandlerResolver = new Mock<IButtonActionHandlerResolver>();
            _buttonActionHandlerResolver
                .Setup(x => x.GetButtonActionHandler(It.IsAny<ButtonSetup>()))
                .Returns(_buttonActionHandler.Object);

            _authService = new Mock<IAuthService>();
            _serviceProvider = new ServiceCollection().AddTransient<DataAnnotationEntityValidator>().BuildServiceProvider();

            _subject = new ButtonInteraction(_collectionResolver.Object, _buttonActionHandlerResolver.Object, _authService.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WhenValidationOfEditorButtonIsRequested_ThenValidityOfEditContextIsTested(bool requiresTesting)
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<ButtonSetup>(), It.IsAny<FormEditContext>())).Returns(requiresTesting);
            var editContext = new FormEditContext("alias", "repo", "entity", new ValidEntity(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            Expression<Func<ValidEntity, string>> property = x => x.Name;
            editContext.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata(property)!);
            var request = new PersistEntityRequestModel()
            {
                ActionId = "123",
                EditContext = editContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.RequiresValidForm(It.IsAny<ButtonSetup>(), It.IsAny<FormEditContext>()), Times.Once());
            Assert.AreEqual(requiresTesting, editContext.WasValidated(PropertyMetadataHelper.GetPropertyMetadata(property)!));
        }

        [Test]
        public void WhenValidationOfEditorButtonIsRequested_ThenExceptionIsThrownIfFormIsInvalid()
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<ButtonSetup>(), It.IsAny<FormEditContext>())).Returns(true);
            var editContext = new FormEditContext("alias", "repo", "entity", new InvalidEntity(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            Expression<Func<InvalidEntity, string>> property = x => x.Name;
            editContext.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata(property)!);
            var request = new PersistEntityRequestModel()
            {
                ActionId = "123",
                EditContext = editContext
            };

            // act & assert
            Assert.ThrowsAsync(typeof(InvalidEntityException), () => _subject.ValidateButtonInteractionAsync(request));
        }

        [Test]
        public void WhenValidationOfEditorButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            var request = new PersistEntityRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                EditContext = editContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickBeforeRepositoryActionAsync(It.IsAny<ButtonSetup>(), It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [Test]
        public void WhenInteractionCompletionOfEditorButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);

            var request = new PersistEntityRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                EditContext = editContext
            };

            // act
            _subject.CompleteButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickAfterRepositoryActionAsync(It.IsAny<ButtonSetup>(), It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenValidityOfEditContextIsTested(bool requiresTesting)
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<ButtonSetup>(), It.IsAny<FormEditContext>())).Returns(requiresTesting);
            var editContext = new FormEditContext("alias", "repo", "entity", new ValidEntity(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider);
            Expression<Func<ValidEntity, string>> property = x => x.Name;
            editContext.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata(property)!);
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = "123",
                ListContext = listContext,
                EditContext = editContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.RequiresValidForm(It.IsAny<ButtonSetup>(), It.IsAny<FormEditContext>()), Times.Once());
            Assert.AreEqual(requiresTesting, editContext.WasValidated(PropertyMetadataHelper.GetPropertyMetadata(property)!));
        }

        [Test]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenExceptionIsThrownIfFormIsInvalid()
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<ButtonSetup>(), It.IsAny<FormEditContext>())).Returns(true);
            var editContext = new FormEditContext("alias", "repo", "entity", new InvalidEntity(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider);
            Expression<Func<InvalidEntity, string>> property = x => x.Name;
            editContext.NotifyPropertyIncludedInForm(PropertyMetadataHelper.GetPropertyMetadata(property)!);
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = "123",
                ListContext = listContext,
                EditContext = editContext
            };

            // act & assert
            Assert.ThrowsAsync(typeof(InvalidEntityException), () => _subject.ValidateButtonInteractionAsync(request));
        }

        [Test]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider);
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                ListContext = listContext,
                EditContext = editContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickBeforeRepositoryActionAsync(It.IsAny<ButtonSetup>(), It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [Test]
        public void WhenValidationOfListButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider);
            var request = new PersistEntitiesRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                ListContext = listContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickBeforeRepositoryActionAsync(It.IsAny<ButtonSetup>(), It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [Test]
        public void WhenInteractionCompletionOfListButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup> { new ValidationSetup(typeof(DataAnnotationEntityValidator), default) }, _serviceProvider);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider);

            var request = new PersistEntitiesRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                ListContext = listContext
            };

            // act
            _subject.CompleteButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickAfterRepositoryActionAsync(It.IsAny<ButtonSetup>(), It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }
    }
}

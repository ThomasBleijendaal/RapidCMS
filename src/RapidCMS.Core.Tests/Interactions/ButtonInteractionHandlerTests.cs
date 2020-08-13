using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Interactions;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Tests.Services.Dispatchers;
using System;
using System.Linq.Expressions;

namespace RapidCMS.Core.Tests.Interactions
{
    public class ButtonInteractionHandlerTests
    {
        private IButtonInteraction _subject = default!;

        private Mock<ISetupResolver<ICollectionSetup>> _collectionResolver = default!;
        private Mock<IButtonActionHandler> _buttonActionHandler = default!;
        private Mock<IButtonActionHandlerResolver> _buttonActionHandlerResolver = default!;
        private Mock<IAuthService> _authService = default!;
        private Mock<IButtonSetup> _button = default!;
        private Mock<ICollectionSetup> _collection = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;

        [SetUp]
        public void Setup()
        {
            _button = new Mock<IButtonSetup>();

            _collection = new Mock<ICollectionSetup>();
            _collection
                .Setup(x => x.FindButton(It.IsAny<string>()))
                .Returns(_button.Object);

            _collectionResolver = new Mock<ISetupResolver<ICollectionSetup>>();
            _collectionResolver
                .Setup(x => x.ResolveSetup(It.IsAny<string>()))
                .Returns(_collection.Object);

            _buttonActionHandler = new Mock<IButtonActionHandler>();
            _buttonActionHandlerResolver = new Mock<IButtonActionHandlerResolver>();
            _buttonActionHandlerResolver
                .Setup(x => x.GetButtonActionHandler(It.IsAny<IButtonSetup>()))
                .Returns(_buttonActionHandler.Object);

            _authService = new Mock<IAuthService>();
            _serviceProvider = new Mock<IServiceProvider>();

            _subject = new ButtonInteraction(_collectionResolver.Object, _buttonActionHandlerResolver.Object, _authService.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WhenValidationOfEditorButtonIsRequested_ThenValidityOfEditContextIsTested(bool requiresTesting)
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<IButton>(), It.IsAny<EditContext>())).Returns(requiresTesting);
            var editContext = new EditContext("alias", "repo", "entity", new ValidEntity(), default, UsageType.Edit, _serviceProvider.Object);
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
            _buttonActionHandler.Verify(x => x.RequiresValidForm(It.IsAny<IButton>(), It.IsAny<EditContext>()), Times.Once());
            Assert.AreEqual(requiresTesting, editContext.WasValidated(PropertyMetadataHelper.GetPropertyMetadata(property)!));
        }

        [Test]
        public void WhenValidationOfEditorButtonIsRequested_ThenExceptionIsThrownIfFormIsInvalid()
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<IButton>(), It.IsAny<EditContext>())).Returns(true);
            var editContext = new EditContext("alias", "repo", "entity", new InvalidEntity(), default, UsageType.Edit, _serviceProvider.Object);
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
            var editContext = new EditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var request = new PersistEntityRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                EditContext = editContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickBeforeRepositoryActionAsync(It.IsAny<IButton>(), It.Is<EditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [Test]
        public void WhenInteractionCompletionOfEditorButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new EditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);

            var request = new PersistEntityRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                EditContext = editContext
            };

            // act
            _subject.CompleteButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickAfterRepositoryActionAsync(It.IsAny<IButton>(), It.Is<EditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenValidityOfEditContextIsTested(bool requiresTesting)
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<IButton>(), It.IsAny<EditContext>())).Returns(requiresTesting);
            var editContext = new EditContext("alias", "repo", "entity", new ValidEntity(), default, UsageType.Edit, _serviceProvider.Object);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider.Object);
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
            _buttonActionHandler.Verify(x => x.RequiresValidForm(It.IsAny<IButton>(), It.IsAny<EditContext>()), Times.Once());
            Assert.AreEqual(requiresTesting, editContext.WasValidated(PropertyMetadataHelper.GetPropertyMetadata(property)!));
        }

        [Test]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenExceptionIsThrownIfFormIsInvalid()
        {
            // arrange
            _buttonActionHandler.Setup(x => x.RequiresValidForm(It.IsAny<IButton>(), It.IsAny<EditContext>())).Returns(true);
            var editContext = new EditContext("alias", "repo", "entity", new InvalidEntity(), default, UsageType.Edit, _serviceProvider.Object);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider.Object);
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
            var editContext = new EditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider.Object);
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
            _buttonActionHandler.Verify(x => x.ButtonClickBeforeRepositoryActionAsync(It.IsAny<IButton>(), It.Is<EditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [Test]
        public void WhenValidationOfListButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new EditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider.Object);
            var request = new PersistEntitiesRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                ListContext = listContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickBeforeRepositoryActionAsync(It.IsAny<IButton>(), It.Is<EditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }

        [Test]
        public void WhenInteractionCompletionOfListButtonIsRequested_ThenHandlerOfButtonIsInvoked()
        {
            // arrange
            var customData = new object();
            var editContext = new EditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var listContext = new ListContext("alias", editContext, default, UsageType.Edit, default, _serviceProvider.Object);

            var request = new PersistEntitiesRequestModel()
            {
                ActionId = "123",
                CustomData = customData,
                ListContext = listContext
            };

            // act
            _subject.CompleteButtonInteractionAsync(request);

            // assert
            _buttonActionHandler.Verify(x => x.ButtonClickAfterRepositoryActionAsync(It.IsAny<IButton>(), It.Is<EditContext>(x => x == editContext), It.Is<ButtonContext>(x => x.CustomData == customData)), Times.Once());
        }
    }
}

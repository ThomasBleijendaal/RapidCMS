using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Interactions;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Tests.Services.Dispatchers;
using System;

namespace RapidCMS.Core.Tests.Interactions
{
    public class ButtonInteractionButtonTests
    {
        private IButtonInteraction _subject = default!;

        private Mock<ICollectionResolver> _collectionResolver = default!;
        private Mock<IAuthService> _authService = default!;
        private Mock<ICollectionSetup> _collection = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;

        [SetUp]
        public void Setup()
        {
            _collection = new Mock<ICollectionSetup>();

            _collectionResolver = new Mock<ICollectionResolver>();
            _collectionResolver
                .Setup(x => x.GetCollection(It.IsAny<string>()))
                .Returns(_collection.Object);

            _authService = new Mock<IAuthService>();
            _serviceProvider = new Mock<IServiceProvider>();

            _subject = new ButtonInteraction(_collectionResolver.Object, _authService.Object);
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenValidationOfEditorButtonIsRequested_ThenUsedButtonShouldBeFetched(string buttonId)
        {
            // arrange
            var request = new PersistEntityRequestModel()
            {
                ActionId = buttonId,
                EditContext = new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _collection.Verify(x => x.FindButton(It.Is<string>(x => x == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedEditorButtonCannotBeFound_ThenExceptionShouldBeThrown(string buttonId)
        {
            // arrange
            var request = new PersistEntityRequestModel()
            {
                ActionId = buttonId,
                EditContext = new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object)
            };
            _collection.Setup(x => x.FindButton(It.IsAny<string>())).Returns(default(ButtonSetup));

            // act & assert
            Assert.ThrowsAsync(typeof(Exception), () => _subject.ValidateButtonInteractionAsync(request));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedEditorButtonCanBeFound_ThenAuthServiceShouldBeConsulted(string buttonId)
        {
            // arrange
            var editContext = new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var request = new PersistEntityRequestModel()
            {
                ActionId = buttonId,
                EditContext = editContext
            };
            _collection.Setup(x => x.FindButton(It.IsAny<string>())).Returns(new ButtonSetup(new DefaultButtonConfig { Id = buttonId }));

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<EditContext>(x => x == editContext), It.Is<ButtonSetup>(x => x.ButtonId == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenUsedButtonShouldBeFetched(string buttonId)
        {
            // arrange
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = buttonId,
                ListContext = new ListContext(
                    "alias",
                    new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _collection.Verify(x => x.FindButton(It.Is<string>(x => x == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedEditorInListButtonCannotBeFound_ThenExceptionShouldBeThrown(string buttonId)
        {
            // arrange
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = buttonId,
                ListContext = new ListContext(
                    "alias",
                    new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };
            _collection.Setup(x => x.FindButton(It.IsAny<string>())).Returns(default(ButtonSetup));

            // act & assert
            Assert.ThrowsAsync(typeof(Exception), () => _subject.ValidateButtonInteractionAsync(request));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedEditorInListButtonCanBeFound_ThenAuthServiceShouldBeConsulted(string buttonId)
        {
            // arrange
            var editContext = new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = buttonId,
                EditContext = editContext,
                ListContext = new ListContext(
                    "alias",
                    new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };
            _collection.Setup(x => x.FindButton(It.IsAny<string>())).Returns(new ButtonSetup(new DefaultButtonConfig { Id = buttonId }));

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<EditContext>(x => x == editContext), It.Is<ButtonSetup>(x => x.ButtonId == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenValidationOfListButtonIsRequested_ThenUsedButtonShouldBeFetched(string buttonId)
        {
            // arrange
            var request = new PersistEntitiesRequestModel()
            {
                ActionId = buttonId,
                ListContext = new ListContext(
                    "alias",
                    new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _collection.Verify(x => x.FindButton(It.Is<string>(x => x == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedListButtonCannotBeFound_ThenExceptionShouldBeThrown(string buttonId)
        {
            // arrange
            var request = new PersistEntitiesRequestModel()
            {
                ActionId = buttonId,
                ListContext = new ListContext(
                    "alias",
                    new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };
            _collection.Setup(x => x.FindButton(It.IsAny<string>())).Returns(default(ButtonSetup));

            // act & assert
            Assert.ThrowsAsync(typeof(Exception), () => _subject.ValidateButtonInteractionAsync(request));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedListButtonCanBeFound_ThenAuthServiceShouldBeConsulted(string buttonId)
        {
            // arrange
            var editContext = new EditContext("alias", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object);
            var request = new PersistEntitiesRequestModel()
            {
                ActionId = buttonId,
                ListContext = new ListContext(
                    "alias",
                    editContext,
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };
            _collection.Setup(x => x.FindButton(It.IsAny<string>())).Returns(new ButtonSetup(new DefaultButtonConfig { Id = buttonId }));

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<EditContext>(x => x == editContext), It.Is<ButtonSetup>(x => x.ButtonId == buttonId)));
        }
    }
}

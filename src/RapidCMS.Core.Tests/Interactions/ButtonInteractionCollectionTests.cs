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
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Tests.Services.Dispatchers;
using System;

namespace RapidCMS.Core.Tests.Interactions
{
    public class ButtonInteractionCollectionTests
    {
        private IButtonInteraction _subject = default!;

        private Mock<ISetupResolver<ICollectionSetup>> _collectionResolver = default!;
        private Mock<IButtonActionHandlerResolver> _buttonActionHandlerResolver = default!;
        private Mock<IAuthService> _authService = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;

        [SetUp]
        public void Setup()
        {
            _collectionResolver = new Mock<ISetupResolver<ICollectionSetup>>();
            _collectionResolver
                .Setup(x => x.ResolveSetupAsync(It.IsAny<string>()))
                .ReturnsAsync((string alias) =>
                    new CollectionSetup(default,
                        default,
                        "name",
                        alias,
                        default,
                        default)
                    {
                        EntityVariant = new EntityVariantSetup("default", default, typeof(DefaultEntityVariant), "alias")
                    });

            _buttonActionHandlerResolver = new Mock<IButtonActionHandlerResolver>();

            _authService = new Mock<IAuthService>();
            _serviceProvider = new Mock<IServiceProvider>();

            _subject = new ButtonInteraction(_collectionResolver.Object, _buttonActionHandlerResolver.Object, _authService.Object);
        }

        [Test]
        public void WhenValidationOfEditorButtonIsRequested_ThenCorrespondingCollectionShouldBeFetched()
        {
            // arrange
            var request = new PersistEntityRequestModel()
            {
                EditContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == "alias")), Times.Once());
        }

        [Test]
        public void WhenInteractionCompletionOfEditorButtonIsRequested_ThenCorrespondingCollectionShouldBeFetched()
        {
            // arrange
            var request = new PersistEntityRequestModel()
            {
                EditContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object)
            };

            // act
            _subject.CompleteButtonInteractionAsync(request);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == "alias")), Times.Once());
        }

        [Test]
        public void WhenValidationOfEditorInListButtonIsRequested_ThenCorrespondingCollectionShouldBeFetched()
        {
            // arrange
            var request = new PersistEntityCollectionRequestModel()
            {
                ListContext = new ListContext(
                    "alias",
                    new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == "alias")), Times.Once());
        }

        [Test]
        public void WhenValidationOfListButtonIsRequested_ThenCorrespondingCollectionShouldBeFetched()
        {
            // arrange
            var request = new PersistEntitiesRequestModel()
            {
                ListContext = new ListContext(
                    "alias",
                    new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == "alias")), Times.Once());
        }

        [Test]
        public void WhenInteractionCompletionOfListButtonIsRequested_ThenCorrespondingCollectionShouldBeFetched()
        {
            // arrange
            var request = new PersistEntitiesRequestModel()
            {
                ListContext = new ListContext(
                    "alias",
                    new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };

            // act
            _subject.CompleteButtonInteractionAsync(request);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == "alias")), Times.Once());
        }
    }
}

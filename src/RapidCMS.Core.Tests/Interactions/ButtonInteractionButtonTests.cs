using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Interactions;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Tests.Services.Dispatchers;
using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Tests.Interactions
{
    public class ButtonInteractionButtonTests
    {
        private IButtonInteraction _subject = default!;

        private Mock<ISetupResolver<CollectionSetup>> _collectionResolver = default!;
        private Mock<IButtonActionHandlerResolver> _buttonActionHandlerResolver = default!;
        private Mock<IAuthService> _authService = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;

        private CollectionSetup _collection = default!;

        [SetUp]
        public void Setup()
        {
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
                        new ButtonSetup
                        {
                            ButtonId = "abc",
                            Buttons = new List<ButtonSetup>()
                        },
                        new ButtonSetup
                        {
                            ButtonId = "def",
                            Buttons = new List<ButtonSetup>()
                        }
                    })
            };

            _collectionResolver = new Mock<ISetupResolver<CollectionSetup>>();
            _collectionResolver
                .Setup(x => x.ResolveSetupAsync(It.IsAny<string>()))
                .ReturnsAsync(_collection);

            _buttonActionHandlerResolver = new Mock<IButtonActionHandlerResolver>();

            _authService = new Mock<IAuthService>();
            _serviceProvider = new Mock<IServiceProvider>();

            _subject = new ButtonInteraction(_collectionResolver.Object, _buttonActionHandlerResolver.Object, _authService.Object);
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedEditorButtonCanBeFound_ThenAuthServiceShouldBeConsulted(string buttonId)
        {
            // arrange
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup>(), _serviceProvider.Object);
            var request = new PersistEntityRequestModel()
            {
                ActionId = buttonId,
                EditContext = editContext
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonSetup>(x => x.ButtonId == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedEditorInListButtonCanBeFound_ThenAuthServiceShouldBeConsulted(string buttonId)
        {
            // arrange
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup>(), _serviceProvider.Object);
            var request = new PersistEntityCollectionRequestModel()
            {
                ActionId = buttonId,
                EditContext = editContext,
                ListContext = new ListContext(
                    "alias",
                    new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup>(), _serviceProvider.Object),
                    default,
                    UsageType.Edit,
                    default,
                    _serviceProvider.Object)
            };

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonSetup>(x => x.ButtonId == buttonId)));
        }

        [TestCase("abc")]
        [TestCase("def")]
        public void WhenUsedListButtonCanBeFound_ThenAuthServiceShouldBeConsulted(string buttonId)
        {
            // arrange
            var editContext = new FormEditContext("alias", "repo", "entity", new DefaultEntityVariant(), default, UsageType.Edit, new List<ValidationSetup>(), _serviceProvider.Object);
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

            // act
            _subject.ValidateButtonInteractionAsync(request);

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<FormEditContext>(x => x == editContext), It.Is<ButtonSetup>(x => x.ButtonId == buttonId)));
        }
    }
}

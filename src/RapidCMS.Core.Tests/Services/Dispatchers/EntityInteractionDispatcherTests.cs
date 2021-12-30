using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Dispatchers.Form;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Navigation;
using RapidCMS.Core.Services.Concurrency;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RapidCMS.Core.Tests.Services.Dispatchers
{
    public class EntityInteractionDispatcherTests
    {
        private IInteractionDispatcher<PersistEntityRequestModel, NodeViewCommandResponseModel> _subject = default!;

        private Mock<IServiceProvider> _serviceProviderMock = default!;

        private Mock<INavigationStateProvider> _navigationStateProvider = default!;
        private Mock<ISetupResolver<ICollectionSetup>> _collectionResolver = default!;
        private Mock<ICollectionSetup> _collection = default!;
        private Mock<IEntityVariantSetup> _entityVariant = default!;
        private Mock<IRepositoryResolver> _repositoryResolver = default!;
        private IConcurrencyService _concurrencyService = default!;
        private Mock<IButtonInteraction> _buttonInteraction = default!;
        private Mock<IEditContextFactory> _editContextFactory = default!;
        private Mock<IMediator> _mediator = default!;

        [SetUp]
        public void Setup()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            _navigationStateProvider = new Mock<INavigationStateProvider>();

            _entityVariant = new Mock<IEntityVariantSetup>();
            _collection = new Mock<ICollectionSetup>();
            _collection
                .Setup(x => x.GetEntityVariant(It.IsAny<IEntity>()))
                .Returns(_entityVariant.Object);
            _collectionResolver = new Mock<ISetupResolver<ICollectionSetup>>();
            _collectionResolver
                .Setup(x => x.ResolveSetupAsync(It.IsAny<string>()))
                .ReturnsAsync(_collection.Object);

            _repositoryResolver = new Mock<IRepositoryResolver>();
            _concurrencyService = new ConcurrencyService(new SemaphoreSlim(1, 1));
            _buttonInteraction = new Mock<IButtonInteraction>();
            _editContextFactory = new Mock<IEditContextFactory>();
            _mediator = new Mock<IMediator>();

            _subject = new EntityInteractionDispatcher(
                _navigationStateProvider.Object,
                _collectionResolver.Object,
                _repositoryResolver.Object,
                _concurrencyService,
                _buttonInteraction.Object,
                _editContextFactory.Object,
                _mediator.Object);
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenCollectionShouldBeResolvedUsingGivenCollectionAlias(string alias)
        {
            // arrange
            var request = new PersistEntityRequestModel
            {
                EditContext = new FormEditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, new List<IValidationSetup>(), _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == alias)));
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenRepositoryShouldBeResolvedByGivenCollection(string alias)
        {
            // arrange
            var request = new PersistEntityRequestModel
            {
                EditContext = new FormEditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, new List<IValidationSetup>(), _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request);

            // assert
            _repositoryResolver.Verify(x => x.GetRepository(It.Is<ICollectionSetup>(x => x == _collection.Object)));
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenCrudTypeShouldBeDeterminedUsingButtonInteraction(string alias)
        {
            // arrange
            var request = new PersistEntityRequestModel
            {
                EditContext = new FormEditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, new List<IValidationSetup>(), _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request);

            // assert
            _buttonInteraction.Verify(x => x.ValidateButtonInteractionAsync(It.Is<IEditorButtonInteractionRequestModel>(x => x == request)));
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenCompletedInteractionShouldBeFlaggedToButtonInteraction(string alias)
        {
            // arrange
            var request = new PersistEntityRequestModel
            {
                EditContext = new FormEditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, new List<IValidationSetup>(), _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request);

            // assert
            _buttonInteraction.Verify(x => x.CompleteButtonInteractionAsync(It.Is<IEditorButtonInteractionRequestModel>(x => x == request)));
        }

        [TestCase(CrudType.View, PageType.Node, UsageType.View)]
        [TestCase(CrudType.Edit, PageType.Node, UsageType.Edit)]
        public void WhenInvokingInteraction_ThenNavigationStateShouldBeDeterminedByButtonCrudType(CrudType crudType, PageType pageType, UsageType usageType)
        {
            // arrange
            var request = new PersistEntityRequestModel
            {
                EditContext = new FormEditContext("alias", "alias", "alias", new DefaultEntityVariant(), default, UsageType.Add, new List<IValidationSetup>(), _serviceProviderMock.Object)
            };
            _buttonInteraction.Setup(x => x.ValidateButtonInteractionAsync(It.IsAny<IEditorButtonInteractionRequestModel>())).ReturnsAsync(crudType);

            // act
            _subject.InvokeAsync(request);

            // assert
            _navigationStateProvider.Verify(
                x => x.AppendNavigationState(null,
                    It.Is<NavigationState>(state =>
                        state.PageType == pageType &&
                        state.UsageType == usageType)));
        }
    }
}

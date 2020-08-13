using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Factories;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Dispatchers.Form;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Services.Concurrency;
using System;
using System.Threading;

namespace RapidCMS.Core.Tests.Services.Dispatchers
{
    public class EntitiesInteractionDispatcherTests
    {
        private IInteractionDispatcher<PersistEntitiesRequestModel, ListViewCommandResponseModel> _subject = default!;

        private Mock<IServiceProvider> _serviceProviderMock = default!;

        private Mock<IPageState> _pageState = default!;

        private Mock<ISetupResolver<ICollectionSetup>> _collectionResolver = default!;
        private Mock<ICollectionSetup> _collection = default!;
        private Mock<IEntityVariantSetup> _entityVariant = default!;
        private Mock<IRepositoryResolver> _repositoryResolver = default!;
        private IConcurrencyService _concurrencyService = default!;
        private Mock<IButtonInteraction> _buttonInteraction = default!;
        private Mock<IEditContextFactory> _editContextFactory = default!;

        [SetUp]
        public void Setup()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            _pageState = new Mock<IPageState>();

            _entityVariant = new Mock<IEntityVariantSetup>();
            _collection = new Mock<ICollectionSetup>();
            _collection
                .Setup(x => x.GetEntityVariant(It.IsAny<IEntity>()))
                .Returns(_entityVariant.Object);
            _collectionResolver = new Mock<ISetupResolver<ICollectionSetup>>();
            _collectionResolver
                .Setup(x => x.ResolveSetup(It.IsAny<string>()))
                .Returns(_collection.Object);

            _repositoryResolver = new Mock<IRepositoryResolver>();
            _concurrencyService = new ConcurrencyService(new SemaphoreSlim(1, 1));
            _buttonInteraction = new Mock<IButtonInteraction>();
            _editContextFactory = new Mock<IEditContextFactory>();

            _subject = new EntitiesInteractionDispatcher(
                _collectionResolver.Object,
                _repositoryResolver.Object,
                _concurrencyService,
                _buttonInteraction.Object,
                _editContextFactory.Object);
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenCollectionShouldBeResolvedUsingGivenCollectionAlias(string alias)
        {
            // arrange
            var request = new PersistEntitiesRequestModel
            {
                ListContext = new ListContext(alias, new EditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, _serviceProviderMock.Object), default, UsageType.Add, default, _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request, _pageState.Object);

            // assert
            _collectionResolver.Verify(x => x.ResolveSetup(It.Is<string>(x => x == alias)));
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenRepositoryShouldBeResolvedByGivenCollection(string alias)
        {
            // arrange
            var request = new PersistEntitiesRequestModel
            {
                ListContext = new ListContext(alias, new EditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, _serviceProviderMock.Object), default, UsageType.Add, default, _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request, _pageState.Object);

            // assert
            _repositoryResolver.Verify(x => x.GetRepository(It.Is<ICollectionSetup>(x => x == _collection.Object)));
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenCrudTypeShouldBeDeterminedUsingButtonInteraction(string alias)
        {
            // arrange
            var request = new PersistEntitiesRequestModel
            {
                ListContext = new ListContext(alias, new EditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, _serviceProviderMock.Object), default, UsageType.Add, default, _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request, _pageState.Object);

            // assert
            _buttonInteraction.Verify(x => x.ValidateButtonInteractionAsync(It.Is<IListButtonInteractionRequestModel>(x => x == request)));
        }

        [TestCase("alias1")]
        [TestCase("alias2")]
        public void WhenInvokingInteraction_ThenCompletedInteractionShouldBeFlaggedToButtonInteraction(string alias)
        {
            // arrange
            var request = new PersistEntitiesRequestModel
            {
                ListContext = new ListContext(alias, new EditContext(alias, alias, alias, new DefaultEntityVariant(), default, UsageType.Add, _serviceProviderMock.Object), default, UsageType.Add, default, _serviceProviderMock.Object)
            };

            // act
            _subject.InvokeAsync(request, _pageState.Object);

            // assert
            _buttonInteraction.Verify(x => x.CompleteButtonInteractionAsync(It.Is<IListButtonInteractionRequestModel>(x => x == request)));
        }
    }
}

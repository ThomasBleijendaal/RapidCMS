using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Dispatchers.Form;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Config;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Services.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RapidCMS.Core.Tests.Services.Dispatchers
{
    // TODO: write tests for refactor of ProcessDataViewAsync

    public class GetEntitiesDispatcherTests
    {
        private IPresentationDispatcher<Models.Request.Form.GetEntitiesRequestModel, ListContext> _subject = default!;

        private Mock<ICollectionResolver> _collectionResolver = default!;
        private Mock<IRepository> _repository = default!;
        private Mock<IRepositoryResolver> _repositoryResolver = default!;
        private Mock<IDataViewResolver> _dataViewResolver = default!;
        private Mock<IParentService> _parentService = default!;
        private IConcurrencyService _concurrencyService = default!;
        private Mock<IAuthService> _authService = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;

        private IEntity _protoEntity = new DefaultEntityVariant();
        private IEnumerable<IEntity> _entities = new List<DefaultEntityVariant>
        {
            new DefaultEntityVariant(),
            new DefaultEntityVariant(),
            new DefaultEntityVariant()
        };

        [SetUp]
        public void Setup()
        {
            _collectionResolver = new Mock<ICollectionResolver>();
            _collectionResolver
                .Setup(x => x.GetCollection(It.IsAny<string>()))
                .Returns((string alias) =>
                    new CollectionSetup(default,
                        "name",
                        alias,
                        default)
                    {
                        EntityVariant = new EntityVariantSetup("default", default, typeof(DefaultEntityVariant), "alias"),
                        SubEntityVariants = new List<IEntityVariantSetup>
                        {
                            new EntityVariantSetup("sub1", default, typeof(SubEntityVariant1), "sub1"),
                            new EntityVariantSetup("sub3", default, typeof(SubEntityVariant3), "sub2"),
                            new EntityVariantSetup("sub2", default, typeof(SubEntityVariant2), "sub3")
                        }
                    });

            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.NewAsync(It.IsAny<IParent>(), It.IsAny<Type>())).ReturnsAsync(_protoEntity);
            _repository.Setup(x => x.GetAllAsync(It.IsAny<IParent>(), It.IsAny<IQuery>())).ReturnsAsync(_entities);
            _repository.Setup(x => x.GetAllNonRelatedAsync(It.IsAny<IRelated>(), It.IsAny<IQuery>())).ReturnsAsync(_entities);
            _repository.Setup(x => x.GetAllRelatedAsync(It.IsAny<IRelated>(), It.IsAny<IQuery>())).ReturnsAsync(_entities);

            _repositoryResolver = new Mock<IRepositoryResolver>();
            _repositoryResolver
                .Setup(x => x.GetRepository(It.IsAny<CollectionSetup>()))
                .Returns(_repository.Object);

            _dataViewResolver = new Mock<IDataViewResolver>();

            _parentService = new Mock<IParentService>();
            _parentService
                .Setup(x => x.GetParentAsync(It.IsAny<ParentPath>()))
                .ReturnsAsync((ParentPath? path) =>
                {
                    var mock = new Mock<IParent>();
                    mock.Setup(x => x.GetParentPath()).Returns(path);
                    return mock.Object;
                });

            _concurrencyService = new ConcurrencyService(new SemaphoreSlim(1, 1));

            _authService = new Mock<IAuthService>();
            _serviceProvider = new Mock<IServiceProvider>();


            _subject = new GetEntitiesDispatcher(
                _collectionResolver.Object,
                _repositoryResolver.Object,
                _dataViewResolver.Object,
                _parentService.Object,
                _concurrencyService,
                _authService.Object,
                _serviceProvider.Object);
        }

        [TestCase("collectionAlias1")]
        [TestCase("collectionAlias2")]
        public void WhenRequestHasCollectionAlias_ThenCollectionResolverIsUsedToFetchCollectionWithThatAlias(string alias)
        {
            // act
            _subject.GetAsync(new Models.Request.Form.GetEntitiesRequestModel { UsageType = UsageType.New, CollectionAlias = alias });

            // assert
            _collectionResolver.Verify(x => x.GetCollection(It.Is<string>(x => x == alias)));
        }

        [TestCase("collectionAlias1")]
        [TestCase("collectionAlias2")]
        public void WhenRequestHasCollectionAlias_ThenRepositoryResolverIsUsedToFetchRepositoryWithCollectionThatHasThatAlias(string alias)
        {
            // act
            _subject.GetAsync(new Models.Request.Form.GetEntitiesRequestModel { UsageType = UsageType.New, CollectionAlias = alias });

            // assert
            _repositoryResolver.Verify(x => x.GetRepository(It.Is<CollectionSetup>(x => x.Alias == alias)));
        }

        [TestCase(default(string))]
        [TestCase("test:1")]
        public void WhenRequestHasParent_ThenParentServiceShouldBeUsedToFetchParent(string? path)
        {
            // act
            _subject.GetAsync(new GetEntitiesOfParentRequestModel { UsageType = UsageType.New, CollectionAlias = "", ParentPath = ParentPath.TryParse(path) });

            // assert
            _parentService.Verify(x => x.GetParentAsync(It.Is<ParentPath>(x => (x == null && path == null) || x!.ToPathString() == path)), Times.Once());
        }

        [Test]
        public void WhenRequestMade_ThenRepositoryShouldBeUsedToFetchProtoEntity()
        {
            // act
            _subject.GetAsync(new Models.Request.Form.GetEntitiesRequestModel { CollectionAlias = "name" });

            // assert
            _repository.Verify(x => x.NewAsync(It.IsAny<IParent>(), It.IsAny<Type>()));
        }

        [TestCase(UsageType.Add)]
        [TestCase(UsageType.Edit)]
        [TestCase(UsageType.List)]
        [TestCase(UsageType.New)]
        [TestCase(UsageType.Node)]
        [TestCase(UsageType.Pick)]
        [TestCase(UsageType.Reordered)]
        [TestCase(UsageType.View)]
        public void WhenRequestMade_ThenAuthServiceShouldBeUsedToValidateActionAgainstProtoEntity(UsageType usageType)
        {
            // act
            _subject.GetAsync(new Models.Request.Form.GetEntitiesRequestModel { CollectionAlias = "name", UsageType = usageType });

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<UsageType>(x => x == usageType), It.Is<IEntity>(x => x == _protoEntity)));
        }

        [TestCase(UsageType.Edit)]
        [TestCase(UsageType.New)]
        [TestCase(UsageType.Node)]
        [TestCase(UsageType.Pick)]
        [TestCase(UsageType.Reordered)]
        [TestCase(UsageType.View)]
        public void WhenRequestingToGetEntitiesOfParent_ThenRepositoryShouldBeQueriedToGetEntities(UsageType usageType)
        {
            // act
            var parentPath = ParentPath.TryParse("path:123");
            var query = new Query();
            _subject.GetAsync(new GetEntitiesOfParentRequestModel { UsageType = usageType, Query = query, CollectionAlias = "alias", ParentPath = parentPath });

            // assert
            _repository.Verify(x => x.GetAllAsync(It.Is<IParent>(x => x.GetParentPath()!.ToPathString() == parentPath!.ToPathString()), It.Is<IQuery>(x => x == query)));
        }

        [TestCase(UsageType.Add)]
        public void WhenRequestingToGetEntitiesToAddToTheRelatedEntity_ThenRepositoryShouldBeQueriedToGetNonRelatedEntities(UsageType usageType)
        {
            // act
            var related = new RelatedEntity(new DefaultEntityVariant());
            var query = new Query();
            _subject.GetAsync(new Models.Request.Form.GetEntitiesOfRelationRequestModel { UsageType = usageType, Query = query, CollectionAlias = "alias", Related = related });

            // assert
            _repository.Verify(x => x.GetAllNonRelatedAsync(It.Is<IRelated>(x => x == related), It.Is<IQuery>(x => x == query)));
        }

        [TestCase(UsageType.Edit)]
        [TestCase(UsageType.New)]
        [TestCase(UsageType.Node)]
        [TestCase(UsageType.Pick)]
        [TestCase(UsageType.Reordered)]
        [TestCase(UsageType.View)]
        public void WhenRequestingToGetEntitiesRelatedToRelatedEntity_ThenRepositoryShouldBeQueriedToGetNonRelatedEntities(UsageType usageType)
        {
            // act
            var related = new RelatedEntity(new DefaultEntityVariant());
            var query = new Query();
            _subject.GetAsync(new Models.Request.Form.GetEntitiesOfRelationRequestModel { UsageType = usageType, Query = query, CollectionAlias = "alias", Related = related });

            // assert
            _repository.Verify(x => x.GetAllRelatedAsync(It.Is<IRelated>(x => x == related), It.Is<IQuery>(x => x == query)));
        }

        [TestCase(UsageType.List)]
        [TestCase(UsageType.NotRoot)]
        [TestCase(UsageType.Root)]
        public void WhenRequestingToPerformUnsupportedAction_ThenInvalidOperationShouldBeThrown(UsageType usageType)
        {
            // act & assert
            Assert.ThrowsAsync(typeof(NotImplementedException), () => _subject.GetAsync(new Models.Request.Form.GetEntitiesRequestModel { UsageType = usageType, CollectionAlias = "alias" }));
        }

        [TestCase(UsageType.Edit, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.Edit, null)]
        [TestCase(UsageType.New, UsageType.Node | UsageType.New, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.Edit)]
        [TestCase(UsageType.View, UsageType.Node | UsageType.View, UsageType.Node | UsageType.View, UsageType.Node | UsageType.View, null)]
        public async Task WhenRequestingToGetEntitiesOfParent_ThenListOfEntitiesShouldBeReturnedWithCorrectUsageTypesAsync(UsageType usageType, UsageType expectedUsageTypes1, UsageType expectedUsageTypes2, UsageType expectedUsageTypes3, UsageType expectedUsageTypes4)
        {
            // act
            var parentPath = ParentPath.TryParse("path:123");
            var query = new Query();
            var response = await _subject.GetAsync(new GetEntitiesOfParentRequestModel { UsageType = usageType, Query = query, CollectionAlias = "alias", ParentPath = parentPath });

            // assert
            Assert.AreEqual(expectedUsageTypes1, response.EditContexts.ElementAtOrDefault(0)?.UsageType ?? 0);
            Assert.AreEqual(expectedUsageTypes2, response.EditContexts.ElementAtOrDefault(1)?.UsageType ?? 0);
            Assert.AreEqual(expectedUsageTypes3, response.EditContexts.ElementAtOrDefault(2)?.UsageType ?? 0);
            Assert.AreEqual(expectedUsageTypes4, response.EditContexts.ElementAtOrDefault(3)?.UsageType ?? 0);
        }

        [TestCase(UsageType.Add, UsageType.Node | UsageType.Pick, UsageType.Node | UsageType.Pick, UsageType.Node | UsageType.Pick, null)]
        public async Task WhenRequestingToGetEntitiesRelatedToRelatedEntity_ThenListOfEntitiesShouldBeReturnedWithCorrectUsageTypesAsync(UsageType usageType, UsageType expectedUsageTypes1, UsageType expectedUsageTypes2, UsageType expectedUsageTypes3, UsageType expectedUsageTypes4)
        {
            // act
            var related = new RelatedEntity(new DefaultEntityVariant());
            var query = new Query();
            var response = await _subject.GetAsync(new Models.Request.Form.GetEntitiesOfRelationRequestModel { UsageType = usageType, Query = query, CollectionAlias = "alias", Related = related });

            // assert
            Assert.AreEqual(expectedUsageTypes1, response.EditContexts.ElementAtOrDefault(0)?.UsageType ?? 0);
            Assert.AreEqual(expectedUsageTypes2, response.EditContexts.ElementAtOrDefault(1)?.UsageType ?? 0);
            Assert.AreEqual(expectedUsageTypes3, response.EditContexts.ElementAtOrDefault(2)?.UsageType ?? 0);
            Assert.AreEqual(expectedUsageTypes4, response.EditContexts.ElementAtOrDefault(3)?.UsageType ?? 0);
        }

        [TestCase(UsageType.List)]
        [TestCase(UsageType.Node)]
        [TestCase(UsageType.NotRoot)]
        [TestCase(UsageType.Reordered)]
        [TestCase(UsageType.Root)]
        public void WhenRequestingUnsupportedUsage_ThenExceptionShouldBeThrown(UsageType usageType)
        {
            // act & assert
            var query = new Query();
            Assert.ThrowsAsync(typeof(NotImplementedException), () => _subject.GetAsync(new Models.Request.Form.GetEntitiesRequestModel { UsageType = usageType, Query = query, CollectionAlias = "alias"}));
        }
    }
}

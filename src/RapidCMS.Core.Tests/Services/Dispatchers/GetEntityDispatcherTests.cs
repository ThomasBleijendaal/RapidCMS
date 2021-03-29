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
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Services.Concurrency;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RapidCMS.Core.Tests.Services.Dispatchers
{
    public class GetEntityDispatcherTests
    {
        private IPresentationDispatcher<GetEntityRequestModel, FormEditContext> _subject = default!;

        private Mock<ISetupResolver<ICollectionSetup>> _collectionResolver = default!;
        private Mock<IRepository> _repository = default!;
        private Mock<IRepositoryResolver> _repositoryResolver = default!;
        private Mock<IParentService> _parentService = default!;
        private IConcurrencyService _concurrencyService = default!;
        private Mock<IAuthService> _authService = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;

        private readonly IEntity _entity = new DefaultEntityVariant();

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
                        $"repo-{alias}",
                        default)
                    {
                        EntityVariant = new EntityVariantSetup("default", default, typeof(DefaultEntityVariant), "defaultentityvariant"),
                        SubEntityVariants = new List<IEntityVariantSetup>
                        {
                            new EntityVariantSetup("sub1", default, typeof(SubEntityVariant1), "subentityvariant1"),
                            new EntityVariantSetup("sub3", default, typeof(SubEntityVariant3), "subentityvariant3"),
                            new EntityVariantSetup("sub2", default, typeof(SubEntityVariant2), "subentityvariant2")
                        }
                    });

            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.NewAsync(It.IsAny<IParent>(), It.IsAny<Type>())).ReturnsAsync(_entity);
            _repository.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<IParent>())).ReturnsAsync(_entity);

            _repositoryResolver = new Mock<IRepositoryResolver>();
            _repositoryResolver
                .Setup(x => x.GetRepository(It.IsAny<CollectionSetup>()))
                .Returns(_repository.Object);
            _parentService = new Mock<IParentService>();

            _concurrencyService = new ConcurrencyService(new SemaphoreSlim(1, 1));

            _authService = new Mock<IAuthService>();
            _serviceProvider = new Mock<IServiceProvider>();

            _subject = new GetEntityDispatcher(
                _collectionResolver.Object,
                _repositoryResolver.Object,
                _parentService.Object,
                _concurrencyService,
                _authService.Object,
                _serviceProvider.Object);
        }

        [TestCase(UsageType.View)]
        [TestCase(UsageType.Edit)]
        public void WhenRequestHasNoIdWhenRequired_ThenExceptionIsThrown(UsageType usageType)
        {
            // act && assert
            Assert.ThrowsAsync(typeof(InvalidOperationException), () => _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType }));
        }

        [TestCase(UsageType.New)]
        public void WhenRequestHasIdWhenNotRequired_ThenExceptionIsThrown(UsageType usageType)
        {
            // act && assert
            Assert.ThrowsAsync(typeof(InvalidOperationException), () => _subject.GetAsync(new GetEntityRequestModel { Id = "something", UsageType = usageType }));
        }

        [TestCase("collectionAlias1")]
        [TestCase("collectionAlias2")]
        public void WhenRequestHasCollectionAlias_ThenCollectionResolverIsUsedToFetchCollectionWithThatAlias(string alias)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = UsageType.New, CollectionAlias = alias });

            // assert
            _collectionResolver.Verify(x => x.ResolveSetupAsync(It.Is<string>(x => x == alias)));
        }

        [TestCase("collectionAlias1")]
        [TestCase("collectionAlias2")]
        public void WhenRequestHasCollectionAlias_ThenRepositoryResolverIsUsedToFetchRepositoryWithCollectionThatHasThatAlias(string alias)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = UsageType.New, CollectionAlias = alias });

            // assert
            _repositoryResolver.Verify(x => x.GetRepository(It.Is<CollectionSetup>(x => x.Alias == alias)));
        }

        [TestCase(default(string))]
        [TestCase("test:1")]
        public void WhenRequestHasParent_ThenParentServiceShouldBeUsedToFetchParent(string? path)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = UsageType.New, CollectionAlias = "", ParentPath = ParentPath.TryParse(path) });

            // assert
            _parentService.Verify(x => x.GetParentAsync(It.Is<ParentPath>(x => (x == null && path == null) || x!.ToPathString() == path)), Times.Once());
        }

        [TestCase(UsageType.View, "1")]
        [TestCase(UsageType.View, "fdsa")]
        [TestCase(UsageType.Edit, "1")]
        [TestCase(UsageType.Edit, "fdsa")]
        public void WhenRequestingToGetEntity_ThenRepositoryShouldBeQueriedToGetEntityById(UsageType usageType, string id)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", Id = id });

            // assert
            _repository.Verify(x => x.GetByIdAsync(It.Is<string>(x => x == id), It.IsAny<IParent>()), Times.Once());
            _repository.VerifyNoOtherCalls();
        }

        [TestCase(UsageType.Add, "1")]
        [TestCase(UsageType.List, "fdsa")]
        [TestCase(UsageType.Pick, "1")]
        [TestCase(UsageType.Reordered, "fdsa")]
        public void WhenRequestingToPerformUnsupportedAction_ThenInvalidOperationShouldBeThrown(UsageType usageType, string id)
        {
            // act & assert
            Assert.ThrowsAsync(typeof(InvalidOperationException), () => _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", Id = id }));
        }

        [TestCase(UsageType.New, "defaultentityvariant", typeof(DefaultEntityVariant))]
        [TestCase(UsageType.New, "subentityvariant1", typeof(SubEntityVariant1))]
        [TestCase(UsageType.New, "subentityvariant2", typeof(SubEntityVariant2))]
        [TestCase(UsageType.New, "subentityvariant3", typeof(SubEntityVariant3))]
        public void WhenRequestingToGetNewEntity_ThenRepositoryShouldBeQueriedToGetNewEntity(UsageType usageType, string variantAlias, Type type)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", VariantAlias = variantAlias });

            // assert
            _repository.Verify(x => x.NewAsync(It.IsAny<IParent>(), It.Is<Type>(x => x == type)), Times.Once());
            _repository.VerifyNoOtherCalls();
        }

        [TestCase(UsageType.View, "1")]
        [TestCase(UsageType.View, "fdsa")]
        [TestCase(UsageType.Edit, "1")]
        [TestCase(UsageType.Edit, "fdsa")]
        public void WhenRequestingToGetEntity_ThenAuthServiceShouldBeConsulted(UsageType usageType, string id)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", Id = id });

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<UsageType>(x => x == usageType), It.Is<IEntity>(x => x == _entity)));
            _authService.VerifyNoOtherCalls();
        }

        [TestCase(UsageType.New, "defaultentityvariant", typeof(DefaultEntityVariant))]
        [TestCase(UsageType.New, "subentityvariant1", typeof(SubEntityVariant1))]
        [TestCase(UsageType.New, "subentityvariant2", typeof(SubEntityVariant2))]
        [TestCase(UsageType.New, "subentityvariant3", typeof(SubEntityVariant3))]
        public void WhenRequestingToGetNewEntity_ThenAuthServiceShouldBeConsulted(UsageType usageType, string variantAlias, Type type)
        {
            // act
            _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", VariantAlias = variantAlias });

            // assert
            _authService.Verify(x => x.EnsureAuthorizedUserAsync(It.Is<UsageType>(x => x == usageType), It.Is<IEntity>(x => x == _entity)));
            _authService.VerifyNoOtherCalls();
        }

        [TestCase(UsageType.View, "1")]
        [TestCase(UsageType.View, "fdsa")]
        [TestCase(UsageType.Edit, "1")]
        [TestCase(UsageType.Edit, "fdsa")]
        public async Task WhenRequestingToGetEntity_ThenEditContextShouldBeReturned(UsageType usageType, string id)
        {
            // act
            var context = await _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", Id = id, VariantAlias = "defaultentityvariant" });

            // assert
            Assert.AreEqual(context.Entity, _entity);
        }

        [TestCase(UsageType.New, "defaultentityvariant", typeof(DefaultEntityVariant))]
        [TestCase(UsageType.New, "subentityvariant1", typeof(SubEntityVariant1))]
        [TestCase(UsageType.New, "subentityvariant2", typeof(SubEntityVariant2))]
        [TestCase(UsageType.New, "subentityvariant3", typeof(SubEntityVariant3))]
        public async Task WhenRequestingToGetNewEntity_ThenEditContextShouldBeReturned(UsageType usageType, string variantAlias, Type type)
        {
            // act
            var context = await _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", VariantAlias = variantAlias });

            // assert
            Assert.AreEqual(context.Entity, _entity);
        }

        [TestCase(UsageType.View, "1")]
        [TestCase(UsageType.View, "fdsa")]
        [TestCase(UsageType.Edit, "1")]
        [TestCase(UsageType.Edit, "fdsa")]
        public void WhenRequestingToGetEntityAndEntityCannotBeFound_ThenExceptionShouldBeThrown(UsageType usageType, string id)
        {
            // arrange
            _repository.Reset();

            // act && assert
            Assert.ThrowsAsync(typeof(Exception), () => _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", Id = id }));
        }

        [TestCase(UsageType.New, "defaultentityvariant", typeof(DefaultEntityVariant))]
        [TestCase(UsageType.New, "subentityvariant1", typeof(SubEntityVariant1))]
        [TestCase(UsageType.New, "subentityvariant2", typeof(SubEntityVariant2))]
        [TestCase(UsageType.New, "subentityvariant3", typeof(SubEntityVariant3))]
        public async Task WhenRequestingToGetNewEntityAndEntityCannotBeCreated_ThenExceptionShouldBeThrown(UsageType usageType, string variantAlias, Type type)
        {
            // act
            var context = await _subject.GetAsync(new GetEntityRequestModel { UsageType = usageType, CollectionAlias = "alias", VariantAlias = variantAlias });

            // assert
            Assert.AreEqual(context.Entity, _entity);
        }
    }
}

using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Services.Concurrency;
using RapidCMS.Core.Services.Parent;
using System.Threading;
using System.Threading.Tasks;

namespace RapidCMS.Core.Tests.Services.Parent;

public class ParentServiceTests
{
    private Mock<IRepository> _repository = default!;
    private Mock<IRepositoryResolver> _repositoryResolver = default!;
    private IConcurrencyService _concurrencyService = default!;
    private IParentService _subject = default!;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IRepository>();
        _repository
            .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<IViewContext>()))
            .ReturnsAsync((string id, IViewContext context) =>
            {
                var mock = new Mock<IEntity>();
                mock.Setup(x => x.Id).Returns(id);
                return mock.Object;
            });
        _repositoryResolver = new Mock<IRepositoryResolver>();

        _concurrencyService = new ConcurrencyService(new SemaphoreSlim(1, 1));
        _subject = new ParentService(_repositoryResolver.Object, _concurrencyService);
    }

    [Test]
    public async Task WhenParentPathIsEmpty_NoParentsAreReturned()
    {
        // arrange

        // act
        var parents = await _subject.GetParentAsync(default);

        // assert
        Assert.That(parents, Is.Null);
    }

    [Test]
    public async Task WhenParentPathContainsOneLevel_OneParentIsReturned()
    {
        // arrange
        _repositoryResolver.Setup(x => x.GetRepository("alias")).Returns(_repository.Object);

        // act
        var parents = await _subject.GetParentAsync(ParentPath.TryParse("alias:123"));

        // assert
        Assert.That(parents!.Entity, Is.Not.Null);
        Assert.That(parents.GetParentPath()!.ToPathString(), Is.EqualTo("alias:123"));
    }
}

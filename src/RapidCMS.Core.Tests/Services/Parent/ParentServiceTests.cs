using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Resolvers;
using RapidCMS.Core.Services;
using RapidCMS.Core.Services.Parent;

namespace RapidCMS.Core.Tests.Services.Parent
{
    public class ParentServiceTests
    {
        private Mock<IRepository> _repository = default!;
        private Mock<IRepositoryResolver> _repositoryResolver = default!;
        private IParentService _subject = default!;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<IParent>())).ReturnsAsync(new Mock<IEntity>().Object);
            _repositoryResolver = new Mock<IRepositoryResolver>();

            _subject = new ParentService(_repositoryResolver.Object);
        }

        [Test]
        public async Task WhenParentPathIsEmpty_NoParentsAreReturned()
        {
            // arrange

            // act
            var parents = await _subject.GetParentAsync(default);

            // assert
            Assert.IsNull(parents);
        }

        [Test]
        public async Task WhenParentPathContainsOneLevel_OneParentIsReturned()
        {
            // arrange
            _repositoryResolver.Setup(x => x.GetRepository("alias")).Returns(_repository.Object);

            // act
            var parents = await _subject.GetParentAsync(ParentPath.TryParse("alias:123"));

            // assert
            Assert.NotNull(parents.Entity);
            Assert.AreEqual("alias:123", parents.GetParentPath().ToPathString());
        }
    }
}

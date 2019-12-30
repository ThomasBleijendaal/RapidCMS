using Moq;
using NUnit.Framework;
using RapidCMS.Core.Interfaces.Data;
using RapidCMS.Core.Interfaces.Repositories;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Tests.Repositories
{
    public class BaseRepositoryTests
    {
        private IRepository _subject = default!;
        private Mock<BaseRepository<string, Entity>> _mock = default!;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<BaseRepository<string, Entity>>();
            _subject = _mock.Object;
        }

        [Test]
        public void WhenEntityIsFetchedById_TheGivenIdIsParsedFirst()
        {
            // act
            _subject.GetByIdAsync("123", null);

            // assert
            _mock.Verify(x => x.ParseKey("123"), Times.Once());
        }

        [Test]
        public void WhenEntityIsInserted_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) => notified = true, default);

            // act
            _subject.InsertAsync(default!);

            // assert
            Assert.IsTrue(notified);
        }

        [Test]
        public void WhenEntityIsUpdated_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) => notified = true, default);

            // act
            _subject.UpdateAsync(default!);

            // assert
            Assert.IsTrue(notified);
        }

        [Test]
        public void WhenEntityIsDeleted_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) => notified = true, default);

            // act
            _subject.DeleteAsync("123", default);

            // assert
            Assert.IsTrue(notified);
        }

        public class Entity : IEntity
        {
            public string? Id { get; set; }
        }
    }
}

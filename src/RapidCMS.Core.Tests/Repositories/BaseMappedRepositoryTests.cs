using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Repositories;
using System;

namespace RapidCMS.Core.Tests.Repositories
{
    public class BaseMappedRepositoryTests
    {
        private IRepository _subject = default!;
        private Mock<IServiceProvider> _serviceProvider = default!;
        private Mock<BaseMappedRepository<Entity, Entity>> _mock = default!;

        [SetUp]
        public void Setup()
        {
            _mock = new Mock<BaseMappedRepository<Entity, Entity>>();
            _serviceProvider = new Mock<IServiceProvider>();
            _subject = _mock.Object;
        }

        [Test]
        public void WhenEntityIsInserted_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) =>
            {
                notified = true;
            }, default);
            var editContext = new EditContext("alias", "repo", "entity", new Entity { Id = "123" }, default, default, _serviceProvider.Object);
            var wrapper = new FormEditContextWrapper<Entity>(editContext);

            // act
            _subject.InsertAsync(wrapper);

            // assert
            Assert.IsTrue(notified);
        }

        [Test]
        public void WhenEntityIsUpdated_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) =>
            {
                notified = true;
            }, default);
            var editContext = new EditContext("alias", "repo", "entity", new Entity { Id = "123" }, default, default, _serviceProvider.Object);
            var wrapper = new FormEditContextWrapper<Entity>(editContext);

            // act
            _subject.InsertAsync(wrapper);

            // assert
            Assert.IsTrue(notified);
        }

        [Test]
        public void WhenEntityIsDeleted_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) =>
            {
                notified = true;
            }, default);

            // act
            _subject.DeleteAsync("123", default);

            // assert
            Assert.IsTrue(notified);
        }

        [Test]
        public void WhenNotifyISCalled_TheChangeTokenIsNotified()
        {
            // arrange
            var notified = false;
            _subject.ChangeToken.RegisterChangeCallback((o) =>
            {
                notified = true;
            }, default);
            
            // act
            _subject.NotifyUpdate();

            // assert
            Assert.IsTrue(notified);
        }

        public class Entity : IEntity
        {
            public string? Id { get; set; }
        }
    }
}

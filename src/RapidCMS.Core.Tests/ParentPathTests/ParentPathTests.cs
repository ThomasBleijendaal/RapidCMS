using NUnit.Framework;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Tests.ParentPathTests
{
    public class ParentPathTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BasicPath()
        {
            // arrange

            // act
            var path = ParentPath.TryParse("test:123");

            // assert
            Assert.AreEqual("test:123", path!.ToPathString());
        }

        [Test]
        public void AppendPath()
        {
            // arrange

            // act
            var path = ParentPath.TryParse("test:123");
            var newPath = ParentPath.AddLevel(path, "test2", "1234");

            // assert
            Assert.AreEqual("test:123", path!.ToPathString());
            Assert.AreEqual("test:123;test2:1234", newPath.ToPathString());
        }

        [Test]
        public void AppendPathFromNull()
        {
            // arrange

            // act
            var path = default(ParentPath);
            var newPath = ParentPath.AddLevel(path, "test2", "1234");

            // assert
            Assert.AreEqual("test2:1234", newPath.ToPathString());
        }

        [Test]
        public void RemoveLevel()
        {
            // arrange

            // act
            var path = ParentPath.TryParse("test:123;test2:1234");
            var (newPath, collectionAlias, id) = ParentPath.RemoveLevel(path);

            // assert
            Assert.AreEqual("test:123;test2:1234", path!.ToPathString());
            Assert.AreEqual("test:123", newPath.ToPathString());
            Assert.AreEqual("test2", collectionAlias);
            Assert.AreEqual("1234", id);
        }

        [Test]
        public void RemoveLevelFromNull()
        {
            // arrange

            // act
            var path = ParentPath.TryParse(null);
            var (newPath, collectionAlias, id) = ParentPath.RemoveLevel(path);

            // assert
            Assert.AreEqual(null, path?.ToPathString());
            Assert.AreEqual("", newPath?.ToPathString());
            Assert.AreEqual(null, collectionAlias);
            Assert.AreEqual(null, id);
        }
    }
}

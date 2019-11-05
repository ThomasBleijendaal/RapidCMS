using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Tests.ParentPathTests
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
            Assert.AreEqual("test:123", path.ToPathString());
        }

        [Test]
        public void AppendPath()
        {
            // arrange

            // act
            var path = ParentPath.TryParse("test:123");
            var newPath = ParentPath.AddLevel(path, "test2", "1234");

            // assert
            Assert.AreEqual("test:123", path.ToPathString());
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
    }
}

using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Tests.ParentPathTests;

public class ParentEntityPathTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BasicPath()
    {
        // arrange
        var parent = new ParentEntity(default, new Entity { Id = "1" }, "test1");

        // act
        var path = parent.GetParentPath();

        // assert
        Assert.That(path!.ToPathString(), Is.EqualTo("test1:1"));
    }

    [Test]
    public void NestedPath()
    {
        // arrange
        var parent = new ParentEntity(
            new ParentEntity(default, new Entity { Id = "2" }, "test2"),
            new Entity { Id = "1" }, "test1");

        // act
        var path = parent.GetParentPath();

        // assert
        Assert.That(path!.ToPathString(), Is.EqualTo("test2:2;test1:1"));
    }

    [Test]
    public void DeepNestedPath()
    {
        // arrange
        var parent = new ParentEntity(
            new ParentEntity(
                new ParentEntity(default, new Entity { Id = "3" }, "test3"),
                new Entity { Id = "2" }, "test2"),
            new Entity { Id = "1" }, "test1");

        // act
        var path = parent.GetParentPath();

        // assert
        Assert.That(path!.ToPathString(), Is.EqualTo("test3:3;test2:2;test1:1"));
    }

    public class Entity : IEntity
    {
        public string Id { get; set; }
    }
}

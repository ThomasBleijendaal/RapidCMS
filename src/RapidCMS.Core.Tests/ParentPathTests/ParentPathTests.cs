using NUnit.Framework;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Tests.ParentPathTests;

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
        Assert.That(path!.ToPathString(), Is.EqualTo("test:123"));
    }

    [Test]
    public void AppendPath()
    {
        // arrange

        // act
        var path = ParentPath.TryParse("test:123");
        var newPath = ParentPath.AddLevel(path, "test2", "1234");

        // assert
        Assert.That(path!.ToPathString(), Is.EqualTo("test:123"));
        Assert.That(newPath.ToPathString(), Is.EqualTo("test:123;test2:1234"));
    }

    [Test]
    public void AppendPathFromNull()
    {
        // arrange

        // act
        var path = default(ParentPath);
        var newPath = ParentPath.AddLevel(path, "test2", "1234");

        // assert
        Assert.That(newPath.ToPathString(), Is.EqualTo("test2:1234"));
    }

    [Test]
    public void RemoveLevel()
    {
        // arrange

        // act
        var path = ParentPath.TryParse("test:123;test2:1234");
        var (newPath, collectionAlias, id) = ParentPath.RemoveLevel(path);

        // assert
        Assert.That(path!.ToPathString(), Is.EqualTo("test:123;test2:1234"));
        Assert.That(newPath.ToPathString(), Is.EqualTo("test:123"));
        Assert.That(collectionAlias, Is.EqualTo("test2"));
        Assert.That(id, Is.EqualTo("1234"));
    }

    [Test]
    public void RemoveLevelRemoveLevel()
    {
        // arrange

        // act
        var path = ParentPath.TryParse("test:123;test2:1234");
        var (intPath, _, _) = ParentPath.RemoveLevel(path);
        var (newPath, collectionAlias, id) = ParentPath.RemoveLevel(intPath);

        // assert
        Assert.That(path!.ToPathString(), Is.EqualTo("test:123;test2:1234"));
        Assert.That(newPath, Is.EqualTo(null));
        Assert.That(collectionAlias, Is.EqualTo("test"));
        Assert.That(id, Is.EqualTo("123"));
    }

    [Test]
    public void RemoveLevelFromNull()
    {
        // arrange

        // act
        var path = ParentPath.TryParse(null);
        var (newPath, collectionAlias, id) = ParentPath.RemoveLevel(path);

        // assert
        Assert.That(path?.ToPathString(), Is.EqualTo(null));
        Assert.That(newPath?.ToPathString(), Is.EqualTo(""));
        Assert.That(collectionAlias, Is.EqualTo(null));
        Assert.That(id, Is.EqualTo(null));
    }
}

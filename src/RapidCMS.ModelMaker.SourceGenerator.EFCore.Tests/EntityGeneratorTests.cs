using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class EntityGeneratorTests
    {
        [Test]
        public void WhenBasicBlogJsonIsRead_ThenBlogEntityIsGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode("./modelmaker.basicblog.json",
                @"using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker
{
    public class Blog : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Title { get; set; }
        
        public System.String Content { get; set; }
    }
}
");
        }
    }
}

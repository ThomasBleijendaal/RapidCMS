using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class EntityGeneratorTests
    {
        [Test]
        public void WhenBasicBlogConfigurationIsRead_ThenBlogEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(
                new[]
                {
                    "./modelmaker.blog.json", 
                    "./modelmaker.category.json" 
                },
                BlogCode.Entity,
                BlogCode.Collection,
                CategoryCode.EntityWithBlog,
                CategoryCode.Collection,
                ContextCode.BlogCategoryContext,
                BlogCode.EntityConfiguration,
                BlogCode.Repository,
                CategoryCode.EntityConfiguration,
                CategoryCode.Repository);
        }
    }
}

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
                    "./modelmaker.category.json",
                    "./modelmaker.blog.json"
                },
                CategoryCode.EntityWithBlog,
                CategoryCode.Collection,
                BlogCode.Entity,
                BlogCode.Collection,
                ContextCode.BlogCategoryContext,
                CategoryCode.EntityConfiguration,
                CategoryCode.Repository,
                BlogCode.EntityConfiguration,
                BlogCode.Repository);
        }
    }
}

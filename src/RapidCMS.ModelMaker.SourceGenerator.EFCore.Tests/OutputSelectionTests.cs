using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class OutputSelectionTests
    {
        [Test]
        public void WhenOnlyCollectionsShouldBeGenerated_ThenOnlyCollectionsAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(new[] { "./modelmaker.category.collection-only.json" }, CategoryCode.Collection);
        }

        [Test]
        public void WhenOnlyContextShouldBeGenerated_ThenOnlyContextAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(new[] { "./modelmaker.category.context-only.json" }, ContextCode.CategoryContext, CategoryCode.EntityConfiguration);
        }

        [Test]
        public void WhenOnlyEntitiesShouldBeGenerated_ThenOnlyEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(new[] { "./modelmaker.category.entity-only.json" }, CategoryCode.EntityWithoutBlog);
        }

        [Test]
        public void WhenOnlyRepositoriesShouldBeGenerated_ThenOnlyRepositoriesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(new[] { "./modelmaker.category.repository-only.json" }, CategoryCode.Repository);
        }
    }
}

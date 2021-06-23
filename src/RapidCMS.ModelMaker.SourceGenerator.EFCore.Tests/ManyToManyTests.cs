using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class ManyToManyTests
    {
        [Test]
        public void WhenManyToManyImplicitConfigurationIsRead_ThenManyToManyEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(
                new[]
                {
                    // Corresponding property in A is not set, making this Many-to-Many implicit
                    "./modelmaker.many-to-many-many-as-implicit.json",
                    "./modelmaker.many-to-many-many-bs.json"
                },
                ManyToManyManyACode.Entity,
                ManyToManyManyACode.Collection,
                ManyToManyManyBCode.Entity,
                ManyToManyManyBCode.Collection,
                ContextCode.ManyToManyContext,
                ManyToManyManyACode.EntityTypeConfiguration,
                ManyToManyManyACode.Repository,
                ManyToManyManyBCode.EntityTypeConfiguration,
                ManyToManyManyBCode.Repository);
        }

        [Test]
        public void WhenManyToManyExplicitConfigurationIsRead_ThenManyToManyEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(
                new[]
                {
                    // Corresponding property in A is set to B, making this Many-to-Many explicit
                    "./modelmaker.many-to-many-many-as-explicit.json",
                    "./modelmaker.many-to-many-many-bs.json"
                },
                ManyToManyManyACode.Entity,
                ManyToManyManyACode.Collection,
                ManyToManyManyBCode.Entity,
                ManyToManyManyBCode.Collection,
                ContextCode.ManyToManyContext,
                ManyToManyManyACode.EntityTypeConfiguration,
                ManyToManyManyACode.Repository,
                ManyToManyManyBCode.EntityTypeConfiguration,
                ManyToManyManyBCode.Repository);
        }
    }
}

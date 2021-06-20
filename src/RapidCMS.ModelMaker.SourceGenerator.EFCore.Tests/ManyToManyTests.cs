using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class ManyToManyTests
    {
        [Test]
        public void WhenManyToManyConfigurationIsRead_ThenManyToManyEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(
                new[]
                {
                    "./modelmaker.many-to-many-many-as.json",
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

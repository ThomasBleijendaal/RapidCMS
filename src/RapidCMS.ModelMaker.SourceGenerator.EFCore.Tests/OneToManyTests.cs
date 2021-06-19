using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class OneToManyTests
    {
        [Test]
        public void WhenOneToManyConfigurationIsRead_ThenOneToManyEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(
                new[]
                {
                    "./modelmaker.one-to-many-manys.json",
                    "./modelmaker.one-to-many-ones.json"
                },
                OneToManyManyCode.Entity,
                OneToManyManyCode.Collection,
                OneToManyOneCode.Entity,
                OneToManyOneCode.Collection,
                ContextCode.OneToManyContext,
                OneToManyManyCode.EntityTypeConfiguration,
                OneToManyManyCode.Repository,
                OneToManyOneCode.EntityTypeConfiguration,
                OneToManyOneCode.Repository);
        }
    }
}

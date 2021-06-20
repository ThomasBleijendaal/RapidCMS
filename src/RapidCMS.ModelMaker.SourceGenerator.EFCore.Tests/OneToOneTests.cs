using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class OneToOneTests
    {
        [Test]
        public void WhenOneToOneImplicitConfigurationIsRead_ThenOneToOneEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(
                new[]
                {
                    // corresponding property in A is set, making it the Depending side of the one-to-one relation between A & B
                    "./modelmaker.one-to-one-one-as.json",
                    "./modelmaker.one-to-one-one-bs-explicit.json"
                },
                OneToOneOneACode.Entity,
                OneToOneOneACode.Collection,
                OneToOneOneBCode.Entity,
                OneToOneOneBCode.Collection,
                ContextCode.OneToOneContext,
                OneToOneOneACode.EntityTypeConfiguration,
                OneToOneOneACode.Repository,
                OneToOneOneBCode.EntityTypeConfiguration,
                OneToOneOneBCode.Repository);
        }

        //[Test]
        //public void WhenoneTooneExplicitConfigurationIsRead_ThenoneTooneEntitiesAreGenerated()
        //{
        //    GeneratorTestHelper.TestGeneratedCode(
        //        new[]
        //        {
        //            "./modelmaker.one-to-one-one-as-explicit.json",
        //            "./modelmaker.one-to-one-one-bs.json"
        //        },
        //        OneToOneOneACode.Entity,
        //        OneToOneOneACode.Collection,
        //        OneToOneOneBCode.Entity,
        //        OneToOneOneBCode.Collection,
        //        ContextCode.OneToOneContext,
        //        OneToOneOneACode.EntityTypeConfiguration,
        //        OneToOneOneACode.Repository,
        //        OneToOneOneBCode.EntityTypeConfiguration,
        //        OneToOneOneBCode.Repository);
        //}
    }
}

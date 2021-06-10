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
                @"using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker
{
    public class Blog : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        [MinLength(1)]
        [MaxLength(127)]
        [RegularExpression(""^[a|b|c]$"")]
        public System.String Title { get; set; }
        
        public System.String Content { get; set; }
        
        [Required]
        public System.Boolean IsPublished { get; set; }
        
        [Required]
        public System.DateTime PublishDate { get; set; }
        
        [Required]
        public int? AuthorId { get; set; }
        public RapidCMS.Example.Shared.Data.Person Author { get; set; }
        
        public ICollection<RapidCMS.Example.Shared.Data.Person> SupportingAuthors { get; set; }
    }
}
");
        }
    }
}

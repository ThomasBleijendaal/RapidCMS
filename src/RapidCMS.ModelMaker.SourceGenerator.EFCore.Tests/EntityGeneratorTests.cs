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
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

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
        
        public ICollection<RapidCMS.Example.Shared.Data.Person> SupportingAuthors { get; set; } = new List<RapidCMS.Example.Shared.Data.Person>();
    }
}
", @"using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

namespace RapidCMS.ModelMaker
{
    public static class BlogCollection
    {
        public static void AddBlogCollection(this ICmsConfig config)
        {
            config.AddCollection<Blog, BaseRepository<Blog>>(
                ""blog"",
                ""Database"",
                ""Cyan30"",
                ""Blog"",
                collection =>
                {
                    collection.SetTreeView(x => x.Title);
                    collection.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);
                        view.AddRow(row =>
                        {
                            row.AddField(x => x.Id.ToString()).SetName(""Id"");
                            row.AddField(x => x.Title);
                            row.AddDefaultButton(DefaultButtonType.Edit);
                            row.AddDefaultButton(DefaultButtonType.Delete);
                        });
                    });
                    collection.SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Up);
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Title).SetType(typeof(RapidCMS.UI.Components.Editors.TextBoxEditor));
                            section.AddField(x => x.Content).SetType(typeof(RapidCMS.UI.Components.Editors.TextAreaEditor));
                            section.AddField(x => x.IsPublished).SetType(typeof(RapidCMS.UI.Components.Editors.DropdownEditor)).SetDataCollection(new RapidCMS.Core.Providers.FixedOptionsDataProvider(new (object, string)[] { (true, ""True""), (false, ""False"") }));
                            section.AddField(x => x.PublishDate).SetType(typeof(RapidCMS.UI.Components.Editors.DateEditor));
                            section.AddField(x => x.AuthorId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPickerEditor)).SetCollectionRelation(""person"");
                            section.AddField(x => x.SupportingAuthors).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPickerEditor)).SetCollectionRelation(""person"");
                        });
                    });
                });
        }
    }
}
");
        }
    }
}

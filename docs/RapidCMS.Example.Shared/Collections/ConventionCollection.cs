using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Collections
{
    public static class ConventionCollection
    {
        // CRUD editor for simple POCO based on conventions 
        public static void AddConventionCollection(this ICmsConfig config)
        {
            config.AddCollection<ConventionalPerson, BaseRepository<ConventionalPerson>>("person-convention", "People (by convention)", collection =>
            {
                collection.SetTreeView(x => x.Name);

                // The convention system resolves the configuration based on the [Display]-attributes placed on the properties of the model of this collection.
                // It uses the EditorTypeHelper.TryFindDefaultEditorType to resolve the best matching editor for the property.
                //
                // - The ListEditor will display a list editor with columns for every column with a [Display] attribute and use its Name and Description
                //   for displaying the name and description.
                //
                // - The ListView+NodeEditor will display a list view with columns for each column with a [Display] attribute with a defined ShortName.
                //   The corresponding node editor will display an editor with fields for each the properties that sport a [Display] attribute, and uses
                //   the Name and Description of said attribute.
                //
                // - The ListView will only display a readonly list view without edit options.
                collection.ConfigureByConvention(CollectionConvention.ListViewNodeEditor);
            });
        }
    }
}

using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Example.Components;
using RapidCMS.Example.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Collections
{
    public static class UserCollection
    {
        // CURD editor with validation attributes, custom editor and custom button panes
        public static void AddUserCollection(this ICmsConfig config)
        {
            // the CMS users https://ionicons.com/, so use the name of any Ion Icon as icon for a collection
            config.AddCollection<User, JsonRepository<User>>("user", icon: "contacts", "Users", collection =>
            {
                collection
                    .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                    .SetListEditor(editor =>
                    {
                        // you can control the number of entities on a single page
                        editor.SetPageSize(1);

                        editor.AddDefaultButton(DefaultButtonType.Return);
                        editor.AddDefaultButton(DefaultButtonType.New);
                        // this pane button opens a sidepane displaying the ResetAllPane Razor component. 
                        // this component must inherit BaseSidePane and allows for more complex flows and confirmations.
                        editor.AddPaneButton(typeof(ResetAllPane), "Reset all passwords", "trash");

                        // custom buttons are also allowed:
                        // editor.AddCustomButton<TActionHandler>(typeof(ButtonType));
                        // they must reference a BaseButton derived Razor component, as well as a ActionHandler which handles
                        // the click from the user. 

                        editor
                            .AddSection(section =>
                            {
                                section.AddField(x => x.Name);
                                section.AddField(x => x.StartDate).SetType(EditorType.Date);

                                // this field uses a custom editor, which must inherit BaseEditor
                                section.AddField(x => x.Password).SetType(typeof(PasswordEditor));

                                section.AddField(x => x.Integer).SetType(EditorType.Numeric);
                                section.AddField(x => x.Double).SetType(EditorType.Numeric);

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                            });
                    });
            });
        }
    }
}

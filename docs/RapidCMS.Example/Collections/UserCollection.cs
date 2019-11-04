using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Config;
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
            config.AddCollection<User>("user", "Users", collection =>
            {
                collection
                    .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                    .SetRepository<JsonRepository<User>>()
                    .SetListEditor(editor =>
                    {
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

                                // this field uses a custom editor, which must inherit BaseEditor
                                section.AddField(x => x.Password).SetType(typeof(PasswordEditor));

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                            });
                    });
            });
        }
    }
}

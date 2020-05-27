using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Example.Shared.Handlers;
using RapidCMS.Repositories;
using RapidCMS.UI.Components.Editors;
using RapidCMS.UI.Components.Preview;

namespace RapidCMS.Example.Shared.Collections
{
    public static class UserCollection
    {
        // CRUD editor with validation attributes, custom editor and custom button panes
        public static void AddUserCollection(this ICmsConfig config)
        {
            // the CMS users https://ionicons.com/, so use the name of any Ion Icon as icon for a collection
            config.AddCollection<User, BaseRepository<User>>("user", icon: "contacts", "Users", collection =>
            {
                collection
                    .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                    .SetListEditor(editor =>
                    {
                        // you can control the number of entities on a single page
                        editor.SetPageSize(2);

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

                                // even though some properties on User are required, saving a User with only its Name set is allowed
                                // since this editor cannot touch those required properties. 
                                // if all displayed properties on a model are valid, the whole model is considered valid, as the user
                                // will be unable to make the model valid otherwise.
                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);

                                section.AddDefaultButton(DefaultButtonType.Edit);
                            });
                    })
                    .SetNodeEditor(editor =>
                    {
                        editor
                            .AddSection(section =>
                            {
                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);

                                section.AddField(x => x.Name);
                                section.AddField(x => x.StartDate).SetType(EditorType.Date);

                                // this field uses a custom editor, which must inherit BaseEditor
                                section.AddField(x => x.Password).SetType(typeof(PasswordEditor));

                                // some default editors (like FileUploadEditor) require custom components, so they must be added using their full classname
                                // NoPreview is a default component indicating this upload editor has no preview of the file
                                //section.AddField(x => x.FileBase64).SetType(typeof(FileUploadEditor<Base64TextFileUploadHandler, NoPreview>))
                                //    .SetName("User file");

                                // ImagePreview is a custom component derived from BasePreview to display the uploaded image
                                //section.AddField(x => x.ProfilePictureBase64).SetType(typeof(FileUploadEditor<Base64ImageUploadHandler, ImagePreview>))
                                //    .SetName("User picture");

                                section.AddField(x => x.Integer).SetType(EditorType.Numeric);
                                section.AddField(x => x.Double).SetType(EditorType.Numeric);

                            });
                    });
            });
        }
    }
}

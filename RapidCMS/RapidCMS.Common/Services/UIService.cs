using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{
    public interface IUIService
    {
        EditorUI GenerateNodeUI(ViewContext viewContext, NodeEditor nodeEditor);
        ListUI GenerateListUI(ViewContext viewContext, ListView listView);
    }

    public class UIService : IUIService
    {
        private readonly Root _root;

        public UIService(Root root)
        {
            _root = root;
        }

        public EditorUI GenerateNodeUI(ViewContext viewContext, NodeEditor nodeEditor)
        {
            return new EditorUI
            {
                Buttons = nodeEditor.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button => button.ToUI()),

                Sections = nodeEditor.EditorPanes
                    .Where(x => x.VariantType == viewContext.EntityVariant.Type || 
                                x.VariantType == viewContext.EntityVariant.Type.BaseType)
                    .ToList(pane =>
                    {
                        var fields = pane.Fields.Select(field =>
                        {
                            return (field.Index, element: (Element)field.ToFieldWithLabelUI());
                        });

                        var subCollections = pane.SubCollectionListEditors.Select(subCollection =>
                        {
                            return (subCollection.Index, element: (Element)subCollection.ToUI());
                        });

                        return new SectionUI
                        {
                            Elements = fields.Union(subCollections)
                                .OrderBy(x => x.Index)
                                .ToList(x => x.element)
                        };
                    })
            };
        }


        public ListUI GenerateListUI(ViewContext viewContext, ListView listView)
        {
            return new ListUI
            {
                Buttons = listView.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button => button.ToUI()),

                Section = listView.ViewPane == null ? null :
                    new SectionUI
                    {
                        Buttons = listView.ViewPane.Buttons
                            .GetAllButtons()
                            .Where(button => button.IsCompatibleWithView(viewContext))
                            .ToList(button => button.ToUI()),

                        Elements = listView.ViewPane.Fields
                            .ToList(field =>
                            {
                                return (Element)field.ToFieldWithLabelUI();

                            })
                    }
            };
        }
    }
}

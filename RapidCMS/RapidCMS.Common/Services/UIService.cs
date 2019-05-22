using System;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{

    internal class UIService : IUIService
    {
        private readonly Root _root;

        public UIService(Root root)
        {
            _root = root;
        }

        public NodeUI GenerateNodeUI(ViewContext viewContext, NodeEditor nodeEditor)
        {
            return new NodeUI
            {
                Buttons = nodeEditor.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithView(viewContext))
                    .ToList(button => button.ToUI()),

                Sections = nodeEditor.EditorPanes
                    .Where(pane => pane.VariantType.IsSameTypeOrBaseTypeOf(viewContext.EntityVariant.Type))
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
                            CustomAlias = pane.CustomAlias,
                            Label = pane.Label,

                            Elements = fields.Union(subCollections)
                                .OrderBy(x => x.Index)
                                .ToList(x => x.element)
                        };
                    })
            };
        }

        public ListUI GenerateListUI(ViewContext listViewContext, Func<UISubject, ViewContext> entityViewContext, ListView listView)
        {
            return new ListUI
            {
                Buttons = listView.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithView(listViewContext))
                    .ToList(button => button.ToUI()),

                SectionForEntity = (entity) => listView.ViewPane == null ? null :
                    new SectionUI
                    {
                        // TODO: custom?
                        // CustomAlias = (listView.ViewPane is CustomEditorPane customEditorPane) ? customEditorPane.Alias : null,

                        Buttons = listView.ViewPane.Buttons
                            .GetAllButtons()
                            .Where(button => button.IsCompatibleWithView(entityViewContext(entity)))
                            .ToList(button => button.ToUI()),

                        Elements = listView.ViewPane.Fields.ToList(field => (Element)field.ToFieldWithLabelUI())
                    }
            };
        }

        public ListUI GenerateListUI(ViewContext listViewContext, Func<UISubject, ViewContext> entityViewContext, ListEditor listEditor)
        {
            return new ListUI
            {
                Buttons = listEditor.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithView(listViewContext))
                    .ToList(button => button.ToUI()),

                SectionForEntity = (subject) =>
                {
                    var pane = listEditor.EditorPanes.FirstOrDefault(pane => pane.VariantType.IsSameTypeOrDerivedFrom(subject.Entity.GetType()));

                    if (pane == null)
                    {
                        return null;
                    }

                    return new SectionUI
                    {
                        CustomAlias = pane.CustomAlias,

                        Buttons = pane.Buttons
                            .GetAllButtons()
                            .Where(button => button.IsCompatibleWithView(entityViewContext(subject)))
                            .ToList(button => button.ToUI()),

                        Elements = pane.Fields.ToList(field => (Element)field.ToFieldWithLabelUI())
                    };
                }
            };
        }
    }
}

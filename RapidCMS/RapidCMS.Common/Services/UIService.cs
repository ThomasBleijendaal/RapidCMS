using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

#nullable enable

namespace RapidCMS.Common.Services
{
    public interface IUIService
    {
        EditorUI GenerateNodeUI(ViewContext viewContext, NodeEditor nodeEditor);
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
    }
}

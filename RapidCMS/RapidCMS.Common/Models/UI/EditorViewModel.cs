using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.UI
{


    public class EditorUI
    {
        private IEntity _entity;

        public void SetEntity(IEntity entity)
        {
            _entity = entity;

            Sections.ForEach(section => section.Elements.ForEach(element =>
            {
                if (element is FieldUI field)
                {
                    field.Entity = _entity;
                }
            }));
        }

        public IEntity GetEntity()
        {
            return _entity;
        }

        public List<ButtonUI> Buttons { get; set; }
        public List<SectionUI> Sections { get; set; }
    }

    public class SectionUI
    {
        public List<Element> Elements { get; set; }
    }

    public class ButtonUI
    {
        public string ButtonId { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public bool ShouldConfirm { get; set; }

        public string Alias { get; set; }
    }

    public class Element
    {

    }

    public class FieldUI : Element
    {
        internal IEntity Entity { get; set; }

        public string Alias { get; set; }

        public EditorType Type { get; set; }

        public IValueMapper ValueMapper { get; set; }
        public PropertyMetadata Property { get; set; }
        public IDataProvider DataProvider { get; set; }

        // TODO: change to object
        public object Value
        {
            get
            {
                return ValueMapper.MapToEditor(null, Property.Getter(Entity));
            }
            set
            {
                Property.Setter(Entity, ValueMapper.MapFromEditor(null, value));
            }
        }

        public string ReadonlyValue
        {
            get
            {
                return ValueMapper.MapToView(null, Property.Getter(Entity));
            }
        }
    }

    public class FieldWithLabelUI : FieldUI
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SubCollectionUI : Element
    {
        public string CollectionAlias { get; set; }
    }
}

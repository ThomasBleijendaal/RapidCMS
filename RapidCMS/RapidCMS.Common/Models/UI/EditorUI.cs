using System.Collections.Generic;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public IEnumerable<IEntity> Entities { get; set; }

        public List<ButtonUI> Buttons { get; set; }
        public SectionUI Section { get; set; }
    }

    public class EditorUI
    {
        public IEntity Entity { get; set; }

        public List<ButtonUI> Buttons { get; set; }
        public List<SectionUI> Sections { get; set; }
    }

    public class SectionUI
    {
        public List<ButtonUI> Buttons { get; set; }
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
        public string Alias { get; set; }

        public EditorType Type { get; set; }

        public IValueMapper ValueMapper { get; set; }
        public PropertyMetadata Property { get; set; }
        public IDataProvider DataProvider { get; set; }

        public object GetValue(IEntity entity)
        {

            return ValueMapper.MapToEditor(null, Property.Getter(entity));
        }

        public void SetValue(IEntity entity, object value)
        {

            Property.Setter(entity, ValueMapper.MapFromEditor(null, value));
        }

        public string GetReadonlyValue(IEntity entity)
        {
            return ValueMapper.MapToView(null, Property.Getter(entity));
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

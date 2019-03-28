using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Interfaces
{
    // TODO: remove context
    public interface IValueMapper
    {
        object MapToEditor(ValueMappingContext context, object value);
        object MapFromEditor(ValueMappingContext context, object value);

        string MapToView(ValueMappingContext context, object value);
    }

    public interface IValueMapper<TModel>
    {
        object MapToEditor(ValueMappingContext context, TModel value);
        TModel MapFromEditor(ValueMappingContext context, object value);

        string MapToView(ValueMappingContext context, TModel value);
    }

    public abstract class ValueMapper<TModel> : IValueMapper, IValueMapper<TModel>
    {
        public abstract TModel MapFromEditor(ValueMappingContext context, object value);
        public abstract object MapToEditor(ValueMappingContext context, TModel value);
        public abstract string MapToView(ValueMappingContext context, TModel value);

        object IValueMapper.MapFromEditor(ValueMappingContext context, object value)
        {
            return MapFromEditor(context, value);
        }

        object IValueMapper.MapToEditor(ValueMappingContext context, object value)
        {
            return MapToEditor(context, (TModel)value);
        }

        string IValueMapper.MapToView(ValueMappingContext context, object value)
        {
            return MapToView(context, (TModel)value);
        }
    }

    public class DefaultValueMapper : ValueMapper<object>
    {
        public override object MapFromEditor(ValueMappingContext context, object value)
        {
            return value;
        }

        public override object MapToEditor(ValueMappingContext context, object value)
        {
            return value?.ToString() ?? string.Empty;
        }

        public override string MapToView(ValueMappingContext context, object value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }

    public class IntValueMapper : ValueMapper<int>
    {
        public override int MapFromEditor(ValueMappingContext context, object value)
        {
            return (int)value;
        }

        public override object MapToEditor(ValueMappingContext context, int value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, int value)
        {
            return value.ToString();
        }
    }

    // TODO: this thing is a bit flaky
    public class CollectionValueMapper<TValue> : ValueMapper<ICollection<TValue>>
    {
        public override ICollection<TValue> MapFromEditor(ValueMappingContext context, object value)
        {
            return (ICollection<TValue>)value;
        }

        public override object MapToEditor(ValueMappingContext context, ICollection<TValue> value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, ICollection<TValue> value)
        {
            return string.Join(", ", value.Select(x => x.ToString()));
        }
    }
}

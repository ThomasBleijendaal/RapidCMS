using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Interfaces
{
    public interface IValueMapper
    {
        string MapToEditor(ValueMappingContext context, object value);
        object MapFromEditor(ValueMappingContext context, string value);

        string MapToView(ValueMappingContext context, object value);
    }

    public interface IValueMapper<TModel>
    {
        string MapToEditor(ValueMappingContext context, TModel value);
        TModel MapFromEditor(ValueMappingContext context, string value);

        string MapToView(ValueMappingContext context, TModel value);
    }

    public abstract class ValueMapper<TModel> : IValueMapper, IValueMapper<TModel>
    {
        public abstract TModel MapFromEditor(ValueMappingContext context, string value);
        public abstract string MapToEditor(ValueMappingContext context, TModel value);
        public abstract string MapToView(ValueMappingContext context, TModel value);

        object IValueMapper.MapFromEditor(ValueMappingContext context, string value)
        {
            return MapFromEditor(context, value);
        }

        string IValueMapper.MapToEditor(ValueMappingContext context, object value)
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
        public override object MapFromEditor(ValueMappingContext context, string value)
        {
            return value;
        }

        public override string MapToEditor(ValueMappingContext context, object value)
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
        public override int MapFromEditor(ValueMappingContext context, string value)
        {
            return int.TryParse(value, out var integer) ? integer : 0;
        }

        public override string MapToEditor(ValueMappingContext context, int value)
        {
            return value.ToString();
        }

        public override string MapToView(ValueMappingContext context, int value)
        {
            return value.ToString();
        }
    }

    // TODO: this should not be needed after UI update
    public class ICollectionValueMapper<TValue> : ValueMapper<ICollection<TValue>>
    {
        public override ICollection<TValue> MapFromEditor(ValueMappingContext context, string value)
        {
            return value.Split(",").Cast<TValue>().ToList();
        }

        public override string MapToEditor(ValueMappingContext context, ICollection<TValue> value)
        {
            return string.Join(",", value.Select(x => x.ToString()));
        }

        public override string MapToView(ValueMappingContext context, ICollection<TValue> value)
        {
            return string.Join(", ", value.Select(x => x.ToString()));
        }
    }
}

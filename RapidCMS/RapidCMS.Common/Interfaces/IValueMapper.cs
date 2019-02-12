using System;
using System.Collections.Generic;
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

    public class DefaultEditorValueMapper : ValueMapper<string>
    {
        public override string MapFromEditor(ValueMappingContext context, string value)
        {
            return value;
        }

        public override string MapToEditor(ValueMappingContext context, string value)
        {
            return value;
        }

        public override string MapToView(ValueMappingContext context, string value)
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultViewValueMapper : ValueMapper<object>
    {
        public override object MapFromEditor(ValueMappingContext context, string value)
        {
            throw new NotImplementedException();
        }

        public override string MapToEditor(ValueMappingContext context, object value)
        {
            throw new NotImplementedException();
        }

        public override string MapToView(ValueMappingContext context, object value)
        {
            return value.ToString();
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
}

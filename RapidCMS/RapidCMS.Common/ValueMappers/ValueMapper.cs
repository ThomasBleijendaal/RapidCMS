using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
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
}

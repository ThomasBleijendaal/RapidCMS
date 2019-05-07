using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
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
}

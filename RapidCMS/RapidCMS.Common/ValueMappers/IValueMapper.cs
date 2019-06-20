namespace RapidCMS.Common.ValueMappers
{
    public interface IValueMapper
    {
        object MapToEditor(object value);
        object MapFromEditor(object value);
    }

    public interface IValueMapper<TModel>
    {
        object MapToEditor(TModel value);
        TModel MapFromEditor(object value);
    }
}

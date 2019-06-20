namespace RapidCMS.Common.ValueMappers
{
    public abstract class ValueMapper<TModel> : IValueMapper, IValueMapper<TModel>
    {
        /// <summary>
        /// Value is usually string, but can be different when used in complex editors.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract TModel MapFromEditor(object value);

        /// <summary>
        /// Returned value should usually be as string, but can be different when used in complex editors.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract object MapToEditor(TModel value);

        object IValueMapper.MapFromEditor(object value)
        {
            return MapFromEditor(value);
        }

        object IValueMapper.MapToEditor(object value)
        {
            return MapToEditor((TModel)value);
        }
    }
}

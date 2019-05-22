using System;
using RapidCMS.Common.Data;

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
        public abstract TModel MapFromEditor(ValueMappingContext context, object value);

        /// <summary>
        /// Returned value should usually be as string, but can be different when used in complex editors.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract object MapToEditor(ValueMappingContext context, TModel value);

        /// <summary>
        /// Always string. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Check if this is obsolete and can be replaced by using IExpressionMetadata.StringGetter")]
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

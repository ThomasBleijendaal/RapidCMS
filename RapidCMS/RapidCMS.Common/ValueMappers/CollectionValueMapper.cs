using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.ValueMappers
{
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
            return value == null ? "" : string.Join(", ", value?.Select(x => x.ToString()));
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Common.ValueMappers
{
    // TODO: this thing is a bit flaky
    public class CollectionValueMapper<TValue> : ValueMapper<ICollection<TValue>>
    {
        public override ICollection<TValue> MapFromEditor(object value)
        {
            return (ICollection<TValue>)value;
        }

        public override object MapToEditor(ICollection<TValue> value)
        {
            return value.Select(x => (object)x).ToList();
        }
    }
}

using System.Collections.Generic;

namespace RapidCMS.Common.Data
{
    public interface IElement
    {
        object Id { get; }

        IEnumerable<string> Labels { get; }
    }
}

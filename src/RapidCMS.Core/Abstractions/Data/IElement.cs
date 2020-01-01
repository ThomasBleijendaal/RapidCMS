using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IElement
    {
        object Id { get; }

        IEnumerable<string> Labels { get; }
    }
}

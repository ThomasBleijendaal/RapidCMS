using System.Collections.Generic;

namespace RapidCMS.Core.Interfaces.Data
{
    public interface IElement
    {
        object Id { get; }

        IEnumerable<string> Labels { get; }
    }
}

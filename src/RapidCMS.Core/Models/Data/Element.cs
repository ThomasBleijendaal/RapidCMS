using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data
{
    public class Element : IElement
    {
        public object Id { get; set; }
        public IEnumerable<string> Labels { get; set; }
    }
}

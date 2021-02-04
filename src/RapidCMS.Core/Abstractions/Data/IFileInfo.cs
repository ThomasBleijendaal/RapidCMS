using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IFileInfo
    {
        string Name { get; }
        Dictionary<string, object> NonStandardProperties { get; }
        long Size { get; }
        string Type { get; }
        long? LastModified { get; }
        DateTime? LastModifiedDate { get; }
    }
}

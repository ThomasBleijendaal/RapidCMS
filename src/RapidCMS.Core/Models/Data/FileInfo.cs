using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Data;

public class FileInfo : IFileInfo
{
    public FileInfo(Tewr.Blazor.FileReader.IFileInfo fileInfo)
    {
        Name = fileInfo.Name;
        NonStandardProperties = fileInfo.NonStandardProperties;
        Size = fileInfo.Size;
        Type = fileInfo.Type;
        LastModified = fileInfo.LastModified;
        LastModifiedDate = fileInfo.LastModifiedDate;
    }

    public string Name { get; internal set; }

    public Dictionary<string, object> NonStandardProperties { get; internal set; }

    public long Size { get; internal set; }

    public string Type { get; internal set; }

    public long? LastModified { get; internal set; }

    public DateTime? LastModifiedDate { get; internal set; }
}

using System;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IFileUploadHandlerConfig
    {
        string Alias { get; }
        Type HandlerType { get; }
    }
}

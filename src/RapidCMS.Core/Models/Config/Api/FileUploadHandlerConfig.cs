using System;
using RapidCMS.Core.Abstractions.Config;

namespace RapidCMS.Core.Models.Config.Api;

internal class FileUploadHandlerConfig : IFileUploadHandlerConfig
{
    public string Alias { get; set; } = default!;
    public Type HandlerType { get; set; } = default!;
}

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Handlers;

public interface IFileUploadHandler
{
    Task<IEnumerable<string>> ValidateFileAsync(IFileInfo fileInfo);

    Task<object> SaveFileAsync(IFileInfo fileInfo, Stream stream);
}

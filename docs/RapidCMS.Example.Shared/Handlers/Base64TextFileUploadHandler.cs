using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blazor.FileReader;
using RapidCMS.Core.Abstractions.Handlers;

namespace RapidCMS.Example.Shared.Handlers
{
    public class Base64TextFileUploadHandler : IFileUploadHandler
    {
        public async Task<object> SaveFileAsync(IFileInfo fileInfo, Stream stream)
        {
            using (stream)
            {
                var buffer = new Memory<byte>(new byte[stream.Length]);
                await stream.ReadAsync(buffer);

                // you'd probably don't want to save the base64 string as property in the model
                // it's better to upload it to some external storage (Azure Blob Storage) and save it's blob name in the model
                // but for this example, it's good enough
                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public IEnumerable<string> ValidateFile(IFileInfo fileInfo)
        {
            // you'd probably want to make this check more thorough as it's trusting completely trusting the user input
            if (fileInfo.Type != "text/plain")
            {
                yield return "Only .txt files are allowed.";
            }

            if (fileInfo.Size > 10 * 1024)
            {
                yield return "Max upload size is 10KB.";
            }
        }
    }
}

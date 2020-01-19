using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.UI.Components.Preview;

namespace RapidCMS.UI.Components.Editors
{
    public partial class FileUploadEditor<TFileUploadHandler, TPreviewComponent> : BasePropertyEditor
        where TFileUploadHandler : IFileUploadHandler
        where TPreviewComponent : BasePreview
    {
        protected ElementReference _fileInput;

        protected double UploadCompletion { get; set; } = 0;

        [Inject]
        protected TFileUploadHandler FileUploadHandler { get; set; } = default!;

        [Inject]
        protected IFileReaderService FileReaderService { get; set; } = default!;

        protected virtual async Task OnFileSelectedAsync(ChangeEventArgs args)
        {
            EditContext.NotifyPropertyBusy(Property);

            UploadCompletion = 0.0;

            var files = await FileReaderService.CreateReference(_fileInput).EnumerateFilesAsync();
            if (files.Count() > 1)
            {
                EditContext.AddValidationMessage(Property, "This editor only supports single files for now.");
            }

            var file = files.FirstOrDefault();
            if (file == null)
            {
                return;
            }

            var fileInfo = await file.ReadFileInfoAsync();
            var validationMessages = FileUploadHandler.ValidateFile(fileInfo);
            if (validationMessages.Any())
            {
                foreach (var message in validationMessages)
                {
                    EditContext.AddValidationMessage(Property, message);
                }

                EditContext.NotifyPropertyFinished(Property);

                StateHasChanged();
                return;
            }

            using var uploadedFile = await UploadFileToTempFileAsync(file, 8192, fileInfo.Size, (completion) =>
            {
                if (completion - UploadCompletion > 5.0)
                {
                    UploadCompletion = completion;
                    StateHasChanged();
                }
            });

            UploadCompletion = 0.0;
            StateHasChanged();

            var value = await FileUploadHandler.SaveFileAsync(fileInfo, uploadedFile);

            SetValueFromObject(value);

            EditContext.NotifyPropertyFinished(Property);
            EditContext.NotifyPropertyChanged(Property);
        }

        protected virtual async Task<Stream> UploadFileToTempFileAsync(
            IFileReference uploadedFile,
            int bufferSize,
            long fileSize,
            Action<double> progressCallback)
        {
            var bufferFileName = Path.GetTempFileName();

            using (var fileHandle = File.OpenWrite(bufferFileName))
            {
                using var fs = await uploadedFile.OpenReadAsync();

                var buffer = new byte[bufferSize];

                int count;
                long totalCount = 0;
                while ((count = await fs.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await fileHandle.WriteAsync(buffer, 0, count);

                    totalCount += count;

                    progressCallback.Invoke(100.0 * totalCount / fileSize);
                }
            }

            return File.OpenRead(bufferFileName);
        }

    }
}

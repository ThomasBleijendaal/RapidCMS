using System;
using System.ComponentModel.DataAnnotations;
using Blazor.FileReader;

namespace RapidCMS.Core.Models.Request.Api
{
    public class UploadFileRequestModel : IFileInfo
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        [Range(1, long.MaxValue)]
        public long Size { get; set; }

        [Required]
        public string Type { get; set; } = default!;

        public long? LastModified { get; set; }

        public DateTime? LastModifiedDate { 
            get => DateTimeOffset.FromUnixTimeMilliseconds(LastModified ?? 0).UtcDateTime;
            set => LastModified = !value.HasValue ? 0 : new DateTimeOffset(value.Value).ToUnixTimeMilliseconds();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class UploadFileModel : IFileInfo
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        [Range(1, long.MaxValue)]
        public long Size { get; set; }

        [Required]
        public string Type { get; set; } = default!;

        public long? LastModified { get; set; }

        public DateTime? LastModifiedDate
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(LastModified ?? 0).UtcDateTime;
            set => LastModified = !value.HasValue ? 0 : new DateTimeOffset(value.Value).ToUnixTimeMilliseconds();
        }

        public Dictionary<string, object> NonStandardProperties { get; set; } = default!;
    }
}

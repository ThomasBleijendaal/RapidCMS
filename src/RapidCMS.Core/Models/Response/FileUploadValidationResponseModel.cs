using System.Collections.Generic;

namespace RapidCMS.Core.Models.Response
{
    public class FileUploadValidationResponseModel
    {
        public IEnumerable<string> ErrorMessages { get; set; } = default!;
    }
}

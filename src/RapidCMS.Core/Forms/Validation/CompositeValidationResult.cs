using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Core.Forms.Validation
{
    public class CompositeValidationResult : ValidationResult
    {
        public string MemberName { get; private set; }
        public List<ValidationResult> Results { get; private set; } = new List<ValidationResult>();

        public CompositeValidationResult(string errorMessage, string memberName) : base(errorMessage) { MemberName = memberName; }

        public void AddResult(ValidationResult validationResult)
        {
            Results.Add(validationResult);
        }
    }
}

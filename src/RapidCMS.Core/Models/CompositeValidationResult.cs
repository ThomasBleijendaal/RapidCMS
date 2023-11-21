using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RapidCMS.Core.Models;

public class CompositeValidationResult : ValidationResult
{
    public string? MemberName { get; private set; }
    public List<ValidationResult> Results { get; private set; } = new List<ValidationResult>();

    public CompositeValidationResult(string errorMessage, string? memberName = default) : base(errorMessage) { MemberName = memberName; }

    public void AddResult(ValidationResult validationResult)
    {
        Results.Add(validationResult);
    }
}

using System;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IValidationSetup
    {
        Type Type { get; }
        object? Configuration { get; }
    }
}

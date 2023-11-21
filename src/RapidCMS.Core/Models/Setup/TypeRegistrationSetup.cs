using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Models.Setup;

public class TypeRegistrationSetup
{
    public Type Type { get; set; } = default!;
    public string Alias { get; set; } = default!;
    public Dictionary<string, object>? Parameters { get; set; } = default!;
}

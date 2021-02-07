using System;

namespace RapidCMS.Example.WebAssembly.FunctionAPI.Authentication
{
    public class AuthenticationConfig
    {
        public Uri Authority { get; set; } = default!;
        public string ValidAudience { get; set; } = default!;
        public string ValidIssuer { get; set; } = default!;
    }
}

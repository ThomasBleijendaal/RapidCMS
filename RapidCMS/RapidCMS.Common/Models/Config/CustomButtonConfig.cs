using System;

namespace RapidCMS.Common.Models.Config
{
    public class CustomButtonConfig : ButtonConfig
    {
        internal string Alias { get; set; }
        internal Action Action { get; set; }
    }
}

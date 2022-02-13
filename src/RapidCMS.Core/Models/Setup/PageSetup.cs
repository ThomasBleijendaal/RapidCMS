using System.Collections.Generic;

namespace RapidCMS.Core.Models.Setup
{
    internal class PageSetup 
    {
        public string Name { get; set; } = default!;
        public string Alias { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public string Color { get; set; } = default!;
        public List<TypeRegistrationSetup> Sections { get; set; } = default!;
    }
}

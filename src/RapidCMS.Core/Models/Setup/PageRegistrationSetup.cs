using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Models.Setup
{
    // TODO: constructor
    internal class PageRegistrationSetup : IPageSetup
    {
        public string Name { get; set; } = default!;
        public string Alias { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public List<ITypeRegistration> Sections { get; set; } = default!;
    }
}

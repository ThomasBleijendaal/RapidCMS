using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RapidCMS.Core.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

[assembly: InternalsVisibleTo("RapidCMS.Repositories")]
[assembly: InternalsVisibleTo("RapidCMS.Example")]

namespace RapidCMS.Core
{
    public class Constants
    {
        public const string New = "new";
        public const string Edit = "edit";
        public const string View = "view";
    }
}

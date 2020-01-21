using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RapidCMS.Core.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

[assembly: InternalsVisibleTo("RapidCMS.Repositories")]
[assembly: InternalsVisibleTo("RapidCMS.Example")]

namespace RapidCMS.Core
{
    public class Constants
    {
        public const string Add = "add";
        public const string Edit = "edit";
        public const string New = "new";
        public const string Pick = "pick";
        public const string View = "view";
    }
}

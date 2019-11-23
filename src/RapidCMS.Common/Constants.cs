using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RapidCMS.Common.Tests")]
[assembly: InternalsVisibleTo("RapidCMS.Repositories")]
[assembly: InternalsVisibleTo("RapidCMS.Example")]

namespace RapidCMS.Common
{
    public static class Constants
    {
        public const string Edit = "edit";
        public const string New = "new";
        public const string Add = "add";
        public const string Pick = "pick";

        public const string List = "list";

        public const string View = "view";
    }
}

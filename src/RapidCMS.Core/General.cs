using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RapidCMS.Core.Tests")]
[assembly: InternalsVisibleTo("RapidCMS.Repositories")]
[assembly: InternalsVisibleTo("RapidCMS.Example")]

namespace RapidCMS.Core
{
    public class Constants
    {
        public const string Edit = "edit";
        public const string View = "view";
    }
}

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RapidCMS.Core.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

[assembly: InternalsVisibleTo("RapidCMS.Repositories")]
[assembly: InternalsVisibleTo("RapidCMS.Api.Core")]
[assembly: InternalsVisibleTo("RapidCMS.Api.Functions")]
[assembly: InternalsVisibleTo("RapidCMS.Api.WebApi")]

[assembly: InternalsVisibleTo("RapidCMS.ModelMaker")]

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

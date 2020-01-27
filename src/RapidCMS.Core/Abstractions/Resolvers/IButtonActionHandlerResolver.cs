using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IButtonActionHandlerResolver
    {
        IButtonActionHandler GetButtonActionHandler(IButtonSetup button);
    }
}

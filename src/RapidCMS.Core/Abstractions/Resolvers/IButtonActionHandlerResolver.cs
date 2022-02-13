using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    internal interface IButtonSetupActionHandlerResolver
    {
        IButtonActionHandler GetButtonActionHandler(ButtonSetup button);
    }
}

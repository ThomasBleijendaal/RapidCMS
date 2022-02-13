using System;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Resolvers.Buttons
{
    internal class ButtonActionHandlerResolver : IButtonSetupActionHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ButtonActionHandlerResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IButtonSetupActionHandler GetButtonActionHandler(ButtonSetup button)
        {
            return _serviceProvider.GetService<IButtonSetupActionHandler>(button.ButtonHandlerType);
        }
    }
}

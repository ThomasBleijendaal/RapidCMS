using System;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;

namespace RapidCMS.Core.Resolvers.Buttons
{
    internal class ButtonActionHandlerResolver : IButtonActionHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ButtonActionHandlerResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IButtonActionHandler GetButtonActionHandler(IButtonSetup button)
        {
            return _serviceProvider.GetService<IButtonActionHandler>(button.ButtonHandlerType);
        }
    }
}

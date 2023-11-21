using System;

namespace RapidCMS.Core.Models.Config;

internal class NavigationButtonConfig : ButtonConfig
{
    internal NavigationButtonConfig(Type handlerType)
    {
        HandlerType = handlerType;
    }

    internal Type HandlerType { get; }
}

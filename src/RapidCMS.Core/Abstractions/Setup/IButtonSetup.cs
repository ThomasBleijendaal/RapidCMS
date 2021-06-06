using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface IButtonSetup : IButton
    {
        string ButtonId { get; }
        IEnumerable<IButtonSetup> Buttons { get; }
        Type? CustomType { get; }
        Type ButtonHandlerType { get; }

        bool IsPrimary { get; }
    }
}

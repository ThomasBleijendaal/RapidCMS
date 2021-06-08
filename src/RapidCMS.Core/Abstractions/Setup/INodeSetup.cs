using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface INodeSetup
    {
        public Type BaseType { get; set; }
        public List<IPaneSetup> Panes { get; set; }
        public List<IButtonSetup> Buttons { get; set; }

        public IButtonSetup? FindButton(string buttonId);
        public IEnumerable<IButtonSetup>? GetAllButtons();
    }
}

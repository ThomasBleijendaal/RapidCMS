
using System;
using System.Collections.Generic;

namespace RapidCMS.Common.Models.Commands
{
    public class ReloadCommand : ViewCommand
    {
        public ReloadCommand()
        {
        }

        public ReloadCommand(string entityId)
        {
            EntityId = new[] { entityId ?? throw new ArgumentNullException(nameof(entityId)) };
        }

        public ReloadCommand(IEnumerable<string> entityIds)
        {
            EntityId = entityIds ?? throw new ArgumentNullException(nameof(entityIds));
        }

        public IEnumerable<string>? EntityId { get; private set; }
    }
}

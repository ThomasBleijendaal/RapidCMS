using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Collections
{
    public static class WorkflowCollection
    {
        public static void AddWorkflowCollection(this ICmsConfig config)
        {
            config.AddWorkflow(workflow =>
            {
                workflow.AddStep<>
            });
        }
    }
}

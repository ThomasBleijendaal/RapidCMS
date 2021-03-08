using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IWorkflowConfig
    {
        IWorkflowConfig AddStep<TEntity>(Action<IWorkflowStepConfig<TEntity>> configure)
            where TEntity : IEntity;
    }

    public interface IWorkflowStepConfig<TEntity> : IHasDisplayPanes<TEntity, IWorkflowStepConfig<TEntity>>
        where TEntity : IEntity
    {

    }
}

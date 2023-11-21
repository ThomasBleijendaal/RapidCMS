using System;
using System.Linq;
using System.Threading;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Shared.Collections;

public static class ActiveCollection
{
    public static void AddActiveCollection(this ICmsConfig config)
    {
        config.AddCollection<Counter, BaseRepository<Counter>>("active", "CommentActive", "Cyan30", "Active Counter", collection =>
        {
            collection
                .SetTreeView(x => x.CurrentCount.ToString())
                .ConfigureByConvention(CollectionConvention.ListViewNodeView);
        });
    }
}

public class CounterRepository : InMemoryRepository<Counter>
{
    private readonly IMediator _mediator;
    private Timer _timer;

    public CounterRepository(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
    {
        GetListForParent(default).Add(new Counter { Id = "1", CurrentCount = 0 });

        _timer = new Timer(x =>
        {
            Increase();

        }, default, 400, 400);
        _mediator = mediator;
    }

    public void Increase()
    {
        GetListForParent(default).First().CurrentCount++;

        _mediator.NotifyEvent(this, new RepositoryEventArgs(GetType(), default, "1", CrudType.Update));
    }
}

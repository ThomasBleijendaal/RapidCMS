using System;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Messages;

namespace RapidCMS.UI.Components.Shared
{
    public abstract class NodesBase : ComponentBase, IHandle<CollectionUpdatedMessage>, IDisposable
    {
        [Inject]
        private IEventAggregator _eventAggregator { get; set; }

        protected override void OnInit()
        {
            _eventAggregator.Subscribe(this);
        }

        public abstract Task HandleAsync(CollectionUpdatedMessage message);

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }
    }
}

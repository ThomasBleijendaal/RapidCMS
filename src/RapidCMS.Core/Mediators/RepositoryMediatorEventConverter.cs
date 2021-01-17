using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Models.EventArgs.Mediators;

namespace RapidCMS.Core.Mediators
{
    internal class RepositoryMediatorEventConverter : IMediatorEventConverter
    {
        private IDisposable? _eventHander;
        private IMediator? _mediator;

        public void RegisterConversion(IMediator mediator)
        {
            _mediator = mediator;
            _eventHander = mediator.RegisterCallback<RepositoryEventArgs>(ConvertRepositoryEventAsync, default);
        }

        private async Task ConvertRepositoryEventAsync(object sender, RepositoryEventArgs args)
        {
            // TODO: convert RepositoryEventArgs to CollectionEventArgs..
        }

        public void Dispose()
        {
            _eventHander?.Dispose();
        }
    }
}

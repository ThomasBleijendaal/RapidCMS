using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.EventArgs.Mediators;

namespace RapidCMS.Core.Mediators
{
    internal class RepositoryMediatorEventConverter : IMediatorEventListener
    {
        private IDisposable? _eventHander;
        private IMediator? _mediator;
        private readonly IRepositoryTypeResolver _repositoryTypeResolver;
        private readonly ICollectionAliasResolver _collectionAliasResolver;

        public RepositoryMediatorEventConverter(
            IRepositoryTypeResolver repositoryTypeResolver,
            ICollectionAliasResolver collectionAliasResolver)
        {
            _repositoryTypeResolver = repositoryTypeResolver;
            _collectionAliasResolver = collectionAliasResolver;
        }

        public void RegisterListener(IMediator mediator)
        {
            _mediator = mediator;
            _eventHander = mediator.RegisterCallback<RepositoryEventArgs>(ConvertRepositoryEventAsync);
        }

        private Task ConvertRepositoryEventAsync(object sender, RepositoryEventArgs args)
        {
            if (_mediator != null)
            {
                var repositoryAlias = _repositoryTypeResolver.GetAlias(args.RepositoryType);
                var collectionAliases = _collectionAliasResolver.GetAlias(repositoryAlias);

                foreach (var collection in collectionAliases)
                {
                    _mediator.NotifyEvent(sender, new CollectionRepositoryEventArgs(collection, repositoryAlias, args.ParentPath, args.Id, args.Action));
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _eventHander?.Dispose();
        }
    }
}

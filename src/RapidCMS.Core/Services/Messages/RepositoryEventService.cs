using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.Core.Services.Messages
{
    internal class RepositoryEventService : IRepositoryEventService
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;

        public RepositoryEventService(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
        }

        public IDisposable SubscribeToRepositoryUpdates(string alias, Func<Task> asyncCallback)
        {
            var collection = _collectionResolver.ResolveSetup(alias);
            return _repositoryResolver.GetRepository(collection).ChangeToken.RegisterChangeCallback((x) => asyncCallback.Invoke(), null);
        }
    }
}

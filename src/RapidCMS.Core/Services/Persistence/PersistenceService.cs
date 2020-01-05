using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Services.Persistence
{
    internal class PersistenceService : IPersistenceService
    {
        private readonly IDispatcher<GetEntityRequestModel, EntityResponseModel> _getEntityDispatcher;
        private readonly IDispatcher<PersistEntityRequestModel, ViewCommandResponseModel> _persistEntityDispatcher;
        private readonly IDispatcher<GetEntitiesRequestModel, EntitiesResponseModel> _getEntitiesDispatcher;
        private readonly IDispatcher<PersistEntitiesRequestModel, ViewCommandResponseModel> _persistEntitiesDispatcher;

        public PersistenceService(
            IDispatcher<GetEntityRequestModel, EntityResponseModel> getEntityDispatcher,
            IDispatcher<PersistEntityRequestModel, ViewCommandResponseModel> persistEntityDispatcher,
            IDispatcher<GetEntitiesRequestModel, EntitiesResponseModel> getEntitiesDispatcher,
            IDispatcher<PersistEntitiesRequestModel, ViewCommandResponseModel> persistEntitiesDispatcher)
        {
            _getEntityDispatcher = getEntityDispatcher;
            _persistEntityDispatcher = persistEntityDispatcher;
            _getEntitiesDispatcher = getEntitiesDispatcher;
            _persistEntitiesDispatcher = persistEntitiesDispatcher;
        }

        public async Task<EditContext> GetEntityAsync(
            UsageType usageType, 
            string collectionAlias,
            string? variantAlias, 
            ParentPath? parentPath,
            string? id)
        {
            var response = await _getEntityDispatcher.InvokeAsync(new GetEntityRequestModel
            {
                CollectionAlias = collectionAlias,
                Id = id,
                ParentPath = parentPath,
                UsageType = usageType,
                VariantAlias = variantAlias
            });

            return response.EditContext;
        }

        public async Task<List<EditContext>> GetEntitiesAsync(
            UsageType usageType, 
            string collectionAlias, 
            ParentPath? parentPath, 
            Query query)
        {
            var response = await _getEntitiesDispatcher.InvokeAsync(new GetEntitiesRequestModel
            {
                CollectionAlias = collectionAlias,
                Query = query,
                ParentPath = parentPath,
                UsageType = usageType
            });

            return response.EditContexts;
        }

        public async Task<ViewCommand> ProcessEntityActionAsync(
            EditContext editContext, 
            string actionId, 
            object? customData)
        {
            var response = await _persistEntityDispatcher.InvokeAsync(new PersistEntityRequestModel
            {
                ActionId = actionId,
                CustomData = customData,
                EditContext = editContext
            });

            return response.ViewCommand;
        }

        public async Task<ViewCommand> ProcessListActionAsync(
            UsageType usageType,
            string collectionAlias,
            ParentPath? parentPath,
            IEnumerable<EditContext> editContexts, 
            string actionId, 
            object? customData)
        {
            var response = await _persistEntitiesDispatcher.InvokeAsync(new PersistChildEntitiesRequestModel
            {
                ActionId = actionId,
                CollectionAlias = collectionAlias,
                CustomData = customData,
                EditContexts = editContexts,
                ParentPath = parentPath,
                UsageType = usageType
            });

            return response.ViewCommand;
        }

        public async Task<ViewCommand> ProcessListActionAsync(
            EditContext editContext, 
            string actionId, 
            object? customData)
        {
            var response = await _persistEntityDispatcher.InvokeAsync(new PersistEntityRequestModel
            {
                ActionId = actionId,
                CustomData = customData,
                EditContext = editContext
            });

            return response.ViewCommand;
        }

        public async Task<ViewCommand> ProcessRelationActionAsync(
            UsageType usageType, 
            string collectionAlias,
            IEnumerable<EditContext> editContexts, 
            string actionId, 
            object? customData)
        {
            var response = await _persistEntitiesDispatcher.InvokeAsync(new PersistEntitiesRequestModel
            {
                ActionId = actionId,
                CollectionAlias = collectionAlias,
                CustomData = customData,
                EditContexts = editContexts,
                UsageType = usageType
            });

            return response.ViewCommand;
        }

        public async Task<ViewCommand> ProcessRelationActionAsync(
            IRelated related,
            EditContext editContext,
            string actionId,
            object? customData)
        {
            var response = await _persistEntityDispatcher.InvokeAsync(new PersistRelatedEntityRequestModel
            {
                ActionId = actionId,
                CustomData = customData,
                EditContext = editContext,
                Related = related
            });

            return response.ViewCommand;
        }
    }
}

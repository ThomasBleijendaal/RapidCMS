using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers;

namespace RapidCMS.ModelMaker.TableStorage
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddModelMakerTableStorage(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse>, RemoveEntityCommandHandler<ModelEntity>>();
            
            services.AddTransient<ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>, GetAllEntitiesCommandHandler<ModelEntity>>();
            services.AddTransient<ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>, GetEntityByIdCommandHandler<ModelEntity>>();
            services.AddTransient<ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>>, GetEntityByAliasCommandHandler<ModelEntity>>();

            services.AddTransient<ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>, InMemoryModelEntityCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>, InMemoryModelEntityCommandHandler>();

            services.AddSingleton(Microsoft.Azure.Cosmos.Table.CloudStorageAccount.DevelopmentStorageAccount);

            return services;
        }
    }
}

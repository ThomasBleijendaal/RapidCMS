using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.ModelMaker.Abstractions.CommandHandlers;
using RapidCMS.ModelMaker.Models.Commands;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Models.Responses;
using RapidCMS.ModelMaker.TableStorage.Abstractions;
using RapidCMS.ModelMaker.TableStorage.CommandHandlers;
using RapidCMS.ModelMaker.TableStorage.Resolvers;

namespace RapidCMS.ModelMaker.TableStorage
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddModelMakerTableStorage(this IServiceCollection services)
        {
            // Model Entity 
            services.AddTransient<ICommandHandler<RemoveRequest<ModelEntity>, ConfirmResponse>, RemoveEntityCommandHandler<ModelEntity>>();
            
            services.AddTransient<ICommandHandler<GetAllRequest<ModelEntity>, EntitiesResponse<ModelEntity>>, GetAllEntitiesCommandHandler<ModelEntity>>();
            services.AddTransient<ICommandHandler<GetByIdRequest<ModelEntity>, EntityResponse<ModelEntity>>, GetEntityByIdCommandHandler<ModelEntity>>();
            services.AddTransient<ICommandHandler<GetByAliasRequest<ModelEntity>, EntityResponse<ModelEntity>>, GetEntityByAliasCommandHandler<ModelEntity>>();

            services.AddTransient<ICommandHandler<InsertRequest<ModelEntity>, EntityResponse<ModelEntity>>, InsertEntityCommandHandler<ModelEntity>>();
            services.AddTransient<ICommandHandler<UpdateRequest<ModelEntity>, ConfirmResponse>, UpdateEntityCommandHandler<ModelEntity>>();
            services.AddTransient<ICommandHandler<PublishRequest<ModelEntity>, ConfirmResponse>, PublishEntityCommandHandler<ModelEntity>>();

            services.AddSingleton<ITableEntityResolver<ModelEntity>, TableEntityResolver<ModelEntity>>();

            // Model Maker Entity
            services.AddTransient<ICommandHandler<RemoveRequest<ModelMakerEntity>, ConfirmResponse>, RemoveEntityCommandHandler<ModelMakerEntity>>();

            services.AddTransient<ICommandHandler<GetAllRequest<ModelMakerEntity>, EntitiesResponse<ModelMakerEntity>>, GetAllEntitiesCommandHandler<ModelMakerEntity>>();
            services.AddTransient<ICommandHandler<GetByIdRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>>, GetEntityByIdCommandHandler<ModelMakerEntity>>();
            services.AddTransient<ICommandHandler<GetByAliasRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>>, GetEntityByAliasCommandHandler<ModelMakerEntity>>();

            services.AddTransient<ICommandHandler<InsertRequest<ModelMakerEntity>, EntityResponse<ModelMakerEntity>>, InsertEntityCommandHandler<ModelMakerEntity>>();
            services.AddTransient<ICommandHandler<UpdateRequest<ModelMakerEntity>, ConfirmResponse>, UpdateEntityCommandHandler<ModelMakerEntity>>();
            services.AddTransient<ICommandHandler<PublishRequest<ModelMakerEntity>, ConfirmResponse>, PublishEntityCommandHandler<ModelMakerEntity>>();

            services.AddSingleton<ITableEntityResolver<ModelMakerEntity>, TableEntityResolver<ModelMakerEntity>>();



            var account = CloudStorageAccount.DevelopmentStorageAccount;

            var tableClient = account.CreateCloudTableClient();
            services.AddSingleton(tableClient);

            return services;
        }
    }
}

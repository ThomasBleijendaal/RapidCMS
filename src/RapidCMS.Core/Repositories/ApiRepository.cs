using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Converters;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.ApiBridge;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.ApiBridge.Response;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Repositories.ApiBridge
{
    public class ApiRepository<TEntity, TCorrespondingRepository> : BaseRepository<TEntity>
        where TEntity : class, IEntity
        where TCorrespondingRepository : IRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public ApiRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializerSettings = new JsonSerializerSettings();

            _jsonSerializerSettings.Converters.Add(new EntityModelJsonConverter<TEntity>());
        }

        public override Task DeleteAsync(string id, IParent? parent)
            => DoRequestAsync(CreateRequest(HttpMethod.Delete, $"entity/{id}", new DeleteModel(parent)));

        public override async Task<IEnumerable<TEntity>> GetAllAsync(IParent? parent, IQuery<TEntity> query)
        {
            var results = await DoRequestAsync<EntitiesModel<TEntity>>(CreateRequest(HttpMethod.Post, "all", new ParentQueryModel(parent, query)));
            if (results == default)
            {
                return Enumerable.Empty<TEntity>();
            }

            query.HasMoreData(results.MoreDataAvailable);

            return results.Entities.Select(x => x.Entity);
        }

        public override async Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRelated related, IQuery<TEntity> query)
        {
            var results = await DoRequestAsync<EntitiesModel<TEntity>>(CreateRequest(HttpMethod.Post, "all/related", new RelatedQueryModel(related, query)));
            if (results == default)
            {
                return Enumerable.Empty<TEntity>();
            }

            query.HasMoreData(results.MoreDataAvailable);

            return results.Entities.Select(x => x.Entity);
        }

        public override async Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRelated related, IQuery<TEntity> query)
        {
            var results = await DoRequestAsync<EntitiesModel<TEntity>>(CreateRequest(HttpMethod.Post, "all/nonrelated", new RelatedQueryModel(related, query)));
            if (results == default)
            {
                return Enumerable.Empty<TEntity>();
            }

            query.HasMoreData(results.MoreDataAvailable);

            return results.Entities.Select(x => x.Entity);
        }

        public override async Task<TEntity?> GetByIdAsync(string id, IParent? parent)
            => (await DoRequestAsync<EntityModel<TEntity>>(CreateRequest(HttpMethod.Post, $"entity/{id}", new ParentQueryModel(parent))))?.Entity;

        public override async Task<TEntity?> InsertAsync(IEditContext<TEntity> editContext)
            => (await DoRequestAsync<EntityModel<TEntity>>(CreateRequest(HttpMethod.Post, "entity", new EditContextModel<TEntity>(editContext))))?.Entity;

        public override async Task<TEntity> NewAsync(IParent? parent, Type? variantType = null)
            => (await DoRequestAsync<EntityModel<TEntity>>(CreateRequest(HttpMethod.Post, "new", new ParentQueryModel(parent, variantType))))?.Entity ?? throw new NotFoundException("Could not create new entity.");

        public override Task UpdateAsync(IEditContext<TEntity> editContext)
            => DoRequestAsync(CreateRequest(HttpMethod.Put, $"entity/{editContext.Entity.Id}", new EditContextModel<TEntity>(editContext)));

        public override Task AddAsync(IRelated related, string id)
            => DoRequestAsync(CreateRequest(HttpMethod.Post, $"relate", new RelateModel(related, id)));

        public override Task RemoveAsync(IRelated related, string id)
            => DoRequestAsync(CreateRequest(HttpMethod.Delete, $"relate", new RelateModel(related, id)));

        public override Task ReorderAsync(string? beforeId, string id, IParent? parent)
            => DoRequestAsync(CreateRequest(HttpMethod.Post, $"reorder", new ReorderModel(beforeId, id, parent)));

        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            return new HttpRequestMessage(method, url);
        }

        private HttpRequestMessage CreateRequest<T>(HttpMethod method, string url, T content)
        {
            if (method == HttpMethod.Get)
            {
                throw new InvalidOperationException();
            }

            var request = CreateRequest(method, url);
            request.Content = new StringContent(JsonConvert.SerializeObject(content, _jsonSerializerSettings), Encoding.UTF8, "application/json");

            return request;
        }

        private async Task<HttpResponseMessage> DoRequestAsync(HttpRequestMessage request)
        {
            var entityType = typeof(TEntity).FullName;
            var repoType = typeof(TCorrespondingRepository).FullName;

            var alias = AliasHelper.GetRepositoryAlias(typeof(ApiRepository<TEntity, TCorrespondingRepository>));

            var httpClient = _httpClientFactory.CreateClient(alias);
            if (httpClient.BaseAddress == default)
            {
                throw new InvalidOperationException($"Please configure an HttpClient for the repository '{alias}' using " +
                    $".{nameof(RapidCMSMiddleware.AddRapidCMSApiRepository)}([..]) and configure its BaseAddress correctly.");
            }

            var response = await httpClient.SendAsync(request);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => response,
                HttpStatusCode.Unauthorized => throw new UnauthorizedAccessException(),
                HttpStatusCode.Forbidden => throw new UnauthorizedAccessException(),
                HttpStatusCode.NotFound => throw new NotFoundException($"{request.RequestUri} not found."),

                _ => throw new InvalidOperationException()
            };
        }

        private async Task<TResult?> DoRequestAsync<TResult>(HttpRequestMessage request)
            where TResult : class
        {
            try
            {
                var response = await DoRequestAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResult>(json, _jsonSerializerSettings);
                return result;
            }
            catch (NotFoundException)
            {
                return default;
            }
        }
    }
}

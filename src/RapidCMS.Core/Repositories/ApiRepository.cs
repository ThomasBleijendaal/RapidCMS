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
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.ApiBridge.Request;
using RapidCMS.Core.Models.ApiBridge.Response;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Repositories.ApiBridge
{
    public class ApiRepository<TEntity> : BaseRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiRepository(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override Task DeleteAsync(IRepositoryContext context, string id, IParent? parent)
            => DoRequestAsync(context, CreateRequest(HttpMethod.Delete, $"entity/{id}", new DeleteModel(parent)));

        public override async Task<IEnumerable<TEntity>> GetAllAsync(IRepositoryContext context, IParent? parent, IQuery<TEntity> query)
        {
            var results = await DoRequestAsync<EntitiesModel<TEntity>>(context, CreateRequest(HttpMethod.Post, "all", new ParentQueryModel(parent, query)));
            if (results == default)
            {
                return Enumerable.Empty<TEntity>();
            }

            query.HasMoreData(results.MoreDataAvailable);

            return results.Entities;
        }

        public override async Task<IEnumerable<TEntity>?> GetAllRelatedAsync(IRepositoryContext context, IRelated related, IQuery<TEntity> query)
        {
            var results = await DoRequestAsync<EntitiesModel<TEntity>>(context, CreateRequest(HttpMethod.Post, "all/related", new RelatedQueryModel(related, query)));
            if (results == default)
            {
                return Enumerable.Empty<TEntity>();
            }

            query.HasMoreData(results.MoreDataAvailable);

            return results.Entities;
        }

        public override async Task<IEnumerable<TEntity>?> GetAllNonRelatedAsync(IRepositoryContext context, IRelated related, IQuery<TEntity> query)
        {
            var results = await DoRequestAsync<EntitiesModel<TEntity>>(context, CreateRequest(HttpMethod.Post, "all/nonrelated", new RelatedQueryModel(related, query)));
            if (results == default)
            {
                return Enumerable.Empty<TEntity>();
            }

            query.HasMoreData(results.MoreDataAvailable);

            return results.Entities;
        }

        public override Task<TEntity?> GetByIdAsync(IRepositoryContext context, string id, IParent? parent)
            => DoRequestAsync<TEntity>(context, CreateRequest(HttpMethod.Post, $"entity/{id}", new ParentQueryModel(parent)));

        public override Task<TEntity?> InsertAsync(IRepositoryContext context, IEditContext<TEntity> editContext)
            => DoRequestAsync<TEntity>(context, CreateRequest(HttpMethod.Post, "entity", new EditContextModel<TEntity>(editContext)));

        public override async Task<TEntity> NewAsync(IRepositoryContext context, IParent? parent, Type? variantType = null)
            => await DoRequestAsync<TEntity>(context, CreateRequest(HttpMethod.Post, "new", new ParentQueryModel(parent, variantType))) ?? throw new NotFoundException("Could not create new entity.");

        public override Task UpdateAsync(IRepositoryContext context, IEditContext<TEntity> editContext)
            => DoRequestAsync(context, CreateRequest(HttpMethod.Put, $"entity/{editContext.Entity.Id}", new EditContextModel<TEntity>(editContext)));

        public override Task AddAsync(IRepositoryContext context, IRelated related, string id)
            => DoRequestAsync(context, CreateRequest(HttpMethod.Post, $"relate", new RelateModel(related, id)));

        public override Task RemoveAsync(IRepositoryContext context, IRelated related, string id)
            => DoRequestAsync(context, CreateRequest(HttpMethod.Delete, $"relate", new RelateModel(related, id)));

        public override Task ReorderAsync(IRepositoryContext context, string? beforeId, string id, IParent? parent)
            => DoRequestAsync(context, CreateRequest(HttpMethod.Post, $"reorder", new ReorderModel(beforeId, id, parent)));

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
            request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            return request;
        }

        private async Task<HttpResponseMessage> DoRequestAsync(IRepositoryContext context, HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(context?.CollectionAlias))
            {
                throw new InvalidOperationException($"ApiRepository's can only be used from contexts where the CollectionAlias is known.");
            }

            var httpClient = _httpClientFactory.CreateClient(context.CollectionAlias);
            if (httpClient.BaseAddress == default)
            {
                throw new InvalidOperationException($"Please configure an HttpClient for the collection '{context.CollectionAlias}' using " +
                    $".{nameof(RapidCMSMiddleware.AddRapidCMSRepositoryApiHttpClient)}([..]) and configure its BaseAddress correctly.");
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

        private async Task<TResult?> DoRequestAsync<TResult>(IRepositoryContext context, HttpRequestMessage request)
            where TResult : class
        {
            try
            {
                var response = await DoRequestAsync(context, request);
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(json);
            }
            catch (NotFoundException)
            {
                return default;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Abstractions.Repositories
{
    public interface IRepository
    {
        Task<IEntity?> GetByIdAsync(IRepositoryContext context, string id, IParent? parent);
        Task<IEnumerable<IEntity>> GetAllAsync(IRepositoryContext context, IParent? parent, IQuery query);

        Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRepositoryContext context, IRelated related, IQuery query);
        Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRepositoryContext context, IRelated related, IQuery query);

        Task<IEntity> NewAsync(IRepositoryContext context, IParent? parent, Type? variantType);
        Task<IEntity?> InsertAsync(IRepositoryContext context, IEditContext editContext);
        Task UpdateAsync(IRepositoryContext context, IEditContext editContext);
        Task DeleteAsync(IRepositoryContext context, string id, IParent? parent);

        Task AddAsync(IRepositoryContext context, IRelated related, string id);
        Task RemoveAsync(IRepositoryContext context, IRelated related, string id);

        Task ReorderAsync(IRepositoryContext context, string? beforeId, string id, IParent? parent);

        IChangeToken ChangeToken { get; }
    }
}

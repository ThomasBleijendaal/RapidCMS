using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Repositories
{
    public interface IRepository
    {
        Task<IEntity?> GetByIdAsync(string id, IParent? parent);
        Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query);

        Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelated related, IQuery query);
        Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelated related, IQuery query);

        /// <summary>
        /// Create a new entity in-memory.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        Task<IEntity> NewAsync(IParent? parent, Type? variantType);
        Task<IEntity?> InsertAsync(EditContext editContext);
        Task UpdateAsync(EditContext editContext);
        Task DeleteAsync(string id, IParent? parent);

        Task AddAsync(IRelated related, string id);
        Task RemoveAsync(IRelated related, string id);

        IChangeToken ChangeToken { get; }
    }
}

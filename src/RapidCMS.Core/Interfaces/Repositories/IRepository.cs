using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Interfaces.Data;

namespace RapidCMS.Core.Interfaces.Repositories
{
    internal interface IRepository
    {
        Task<IEntity?> GetByIdAsync(string id, IParent? parent);
        Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query);

        // TODO: replace with IRelated
        Task<IEnumerable<IEntity>> GetAllRelatedAsync(IEntity relatedEntity, IQuery query);
        Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IEntity relatedEntity, IQuery query);

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

        Task AddAsync(IEntity relatedEntity, string id);
        Task RemoveAsync(IEntity relatedEntity, string id);

        IChangeToken ChangeToken { get; }
    }
}

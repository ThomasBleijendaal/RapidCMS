using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Abstractions.Repositories
{
    /// <summary>
    /// This interface is used everywhere there is data needed. 
    /// 
    /// This is not the best starting point for custom developed repositories, it is better to see whether the repository can be developed using BaseRepository / BaseMappedRespository as base type.
    /// </summary>
    public interface IRepository
    {
        Task<IEntity?> GetByIdAsync(string id, IViewContext viewContext);
        Task<IEnumerable<IEntity>> GetAllAsync(IViewContext viewContext, IView view);

        Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelatedViewContext viewContext, IView view);
        Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelatedViewContext viewContext, IView view);

        Task<IEntity> NewAsync(IViewContext viewContext, Type? variantType);
        Task<IEntity?> InsertAsync(IEditContext editContext);
        Task UpdateAsync(IEditContext editContext);
        Task DeleteAsync(string id, IViewContext viewContext);

        Task AddAsync(IRelatedViewContext viewContext, string id);
        Task RemoveAsync(IRelatedViewContext viewContext, string id);

        Task ReorderAsync(string? beforeId, string id, IViewContext viewContext);
    }
}

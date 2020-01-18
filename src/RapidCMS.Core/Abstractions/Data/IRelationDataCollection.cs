using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IRelationDataCollection : IDataCollection
    {
        Task<IReadOnlyList<IElement>> GetRelatedElementsAsync();

        Task AddElementAsync(IElement option);
        Task RemoveElementAsync(IElement option);

        IReadOnlyList<IElement> GetCurrentRelatedElements();

        Type GetRelatedEntityType();
    }
}

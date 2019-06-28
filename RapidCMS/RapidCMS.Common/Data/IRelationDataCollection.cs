using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    public interface IRelationDataCollection : IDataCollection
    {
        Task SetRelationMetadataAsync(IEntity entity, IPropertyMetadata collectionProperty);

        Task<IEnumerable<IElement>> GetRelatedElementsAsync();

        Task AddElementAsync(IElement option);
        Task RemoveElementAsync(IElement option);

        IEnumerable<IElement> GetCurrentRelatedElements();

        Type GetRelatedEntityType();
    }
}

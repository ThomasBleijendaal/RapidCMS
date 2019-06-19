using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Data
{
    // TODO: paginate etc
    // TODO: clean API bit up (SetEntityAsync and SetRelationMetadataAsync do almost the same)
    public interface IDataCollection
    {
        Task SetEntityAsync(IEntity entity);
        Task<IEnumerable<IElement>> GetAvailableElementsAsync();
    }

    public interface IRelationDataCollection : IDataCollection
    {
        void SetElementMetadata(IRepository repository, Type relatedEntityType, IPropertyMetadata? repositoryParentIdProperty, IPropertyMetadata idProperty, IExpressionMetadata labelProperty);
        Task SetRelationMetadataAsync(IEntity entity, IPropertyMetadata collectionProperty);

        Task<IEnumerable<IElement>> GetRelatedElementsAsync();

        Task AddElementAsync(IElement option);
        Task RemoveElementAsync(IElement option);

        IEnumerable<IElement> GetCurrentRelatedElements();

        Type GetRelatedEntityType();
    }
}

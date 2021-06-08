using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IRelationDataCollection : IDataCollection
    {
        Task<IReadOnlyList<IElement>> GetRelatedElementsAsync();

        void AddElement(object id);
        void RemoveElement(object id);

        bool IsRelated(object id);

        IReadOnlyList<object> GetCurrentRelatedElementIds();

        Type GetRelatedEntityType();
    }
}

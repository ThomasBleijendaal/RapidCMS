using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Data
{
    // TODO: check when this object is disposed
    public interface IDataCollection : IDisposable
    {
        /// <summary>
        /// Use this method to receive the EditContext for which this data collection is used. This allows for making the available elements contextual to the entity.
        /// </summary>
        /// <param name="editContext">EditContext for which this data collection is used.</param>
        /// <param name="parent">The parent(s) of the entity.</param>
        /// <returns></returns>
        Task SetEntityAsync(FormEditContext editContext, IParent? parent);

        /// <summary>
        /// This method is called when the editor which this data collection requests elements to display.
        /// </summary>
        /// <param name="query">A query for filtering the elements. Not all editors will issue meaningful queries.</param>
        /// <returns></returns>
        Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IQuery query);

        /// <summary>
        /// The editor using this data collection will refresh its UI when this event is invoked. Use this to refresh the UI when the available elements changes.
        /// </summary>
        event EventHandler OnDataChange;
    }
}

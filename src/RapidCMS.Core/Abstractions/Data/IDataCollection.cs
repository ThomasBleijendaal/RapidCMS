using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Data
{
    public interface IDataCollection : IDisposable
    {
        /// <summary>
        /// This method is called after construction when a configuration object was provided using SetDataCollection&lt;TDataCollection, TConfig&gt;(TConfig).
        /// </summary>
        /// <param name="configuration"></param>
        void Configure(object configuration);

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
        /// <param name="view">A query for filtering the elements. Not all editors will issue meaningful queries.</param>
        /// <returns></returns>
        Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IView view);

        /// <summary>
        /// The editor using this data collection will refresh its UI when this event is invoked. Use this to refresh the UI when the available elements changes.
        /// </summary>
        event EventHandler OnDataChange;
    }
}

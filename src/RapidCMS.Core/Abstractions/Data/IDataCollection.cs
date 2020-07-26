using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Abstractions.Data
{
    // TODO: paginate, search etc
    public interface IDataCollection
    {
        /// <summary>
        /// Use this method to receive the EditContext for which this data collection is used. This allows for making the available elements contextual to the entity.
        /// </summary>
        /// <param name="editContext">EditContext for which this data collection is used.</param>
        /// <param name="parent">The parent(s) of the entity.</param>
        /// <returns></returns>
        Task SetEntityAsync(EditContext editContext, IParent? parent);

        /// <summary>
        /// This method is called when the editor which this data collection requests elements to display.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IElement>> GetAvailableElementsAsync();

        /// <summary>
        /// The editor using this data collection will refresh its UI when this event is invoked. Use this to refresh the UI when the available elements changes.
        /// </summary>
        event EventHandler OnDataChange;
    }
}

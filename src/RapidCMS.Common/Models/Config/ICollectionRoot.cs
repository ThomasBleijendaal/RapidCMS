using System.Collections.Generic;

namespace RapidCMS.Common.Models.Config
{
    public interface ICollectionConfig
    {
        /// <summary>
        /// Collections known to this node in the collection tree.
        /// </summary>
        List<ICollectionConfig> Collections { get; set; }

        /// <summary>
        /// Verifies if the given alias is unique.
        /// </summary>
        /// <param name="alias">Alias of a collection</param>
        /// <returns></returns>
        bool IsUnique(string alias);

        string Alias { get; }
    }
}

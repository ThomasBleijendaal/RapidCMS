using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IApiCollectionConfig
    {
        /// <summary>
        /// Adds a data view builder to the collection. Data view builders allow for creating dynamic data views.
        /// </summary>
        /// <typeparam name="TDataViewBuilder"></typeparam>
        /// <returns></returns>
        IApiCollectionConfig SetDataViewBuilder<TDataViewBuilder>() where TDataViewBuilder : IDataViewBuilder;
    }
}

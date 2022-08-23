using System;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IHasConfigurability<TEntity, TReturn>
    {
        /// <summary>
        /// Sets configuration for the field. This will be made available in the component as Configuration property.
        /// Use IWantConfiguration and IRequireConfiguration interfaces to gain access to the GetConfig() extension method.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        TReturn SetConfiguration<TConfig>(TConfig config) where TConfig : class;

        /// <summary>
        /// Sets configuration for the field. This will be made available in the component as Configuration property.
        /// Use IWantConfiguration and IRequireConfiguration interfaces to gain access to the GetConfig() extension method.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        TReturn SetConfiguration<TConfig>(Func<TEntity, EntityState, TConfig?> config) where TConfig : class;

        /// <summary>
        /// Sets configuration for the field. This will be made available in the component as Configuration property.
        /// Use IWantConfiguration and IRequireConfiguration interfaces to gain access to the GetConfig() extension method.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        TReturn SetConfiguration<TConfig>(Func<TEntity, EntityState, Task<TConfig?>> config) where TConfig : class;
    }
}

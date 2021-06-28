namespace RapidCMS.Core.Abstractions.Config
{
    public interface IHasConfigurability<TReturn>
    {
        /// <summary>
        /// Sets configuration for the field. This will be made available in the component as Configuration property.
        /// </summary>
        /// <typeparam name="TConfig"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        TReturn SetConfiguration<TConfig>(TConfig config);
    }
}

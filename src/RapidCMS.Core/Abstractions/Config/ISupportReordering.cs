namespace RapidCMS.Core.Abstractions.Config
{
    public interface ISupportReordering<TReturn>
    {
        /// <summary>
        /// Allows entities to be reordered in the ListEditor.
        /// </summary>
        /// <returns></returns>
        TReturn AllowReordering(bool allowReordering);
    }
}

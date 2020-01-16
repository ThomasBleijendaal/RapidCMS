namespace RapidCMS.Core.Abstractions.Config
{
    public interface ISupportReordering<TReturn>
    {
        /// <summary>
        /// Allows entities to be reorderd in the ListEditor.
        /// </summary>
        /// <returns></returns>
        TReturn AllowReordering(bool allowReordering);
    }
}

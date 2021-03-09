using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IHasColumnVisibility<TReturn>
    {
        /// <summary>
        /// Controls whether empty columns in the table should be collapsed. Only required when the
        /// collection uses multiple EntityVariants, with seperate sets of properties which are not shared between the variants. Collapsing
        /// the empty cell will reduce the number of columns required, and makes the table more readable.
        /// </summary>
        /// <param name="columnVisibility"></param>
        /// <returns></returns>
        TReturn SetColumnVisibility(EmptyVariantColumnVisibility columnVisibility);
    }
}

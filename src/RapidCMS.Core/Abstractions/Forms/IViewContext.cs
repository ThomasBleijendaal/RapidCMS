using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Forms
{
    public interface IViewContext
    {
        /// <summary>
        /// Alias of the collection this view is used
        /// </summary>
        string CollectionAlias { get; }

        /// <summary>
        /// Possible parent(s) of the subject
        /// </summary>
        IParent? Parent { get; }
    }
}

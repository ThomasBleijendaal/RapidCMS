using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Abstractions.Forms;

public interface IRelatedViewContext : IViewContext
{
    /// <summary>
    /// The related entity
    /// </summary>
    IRelated Related { get; }
}

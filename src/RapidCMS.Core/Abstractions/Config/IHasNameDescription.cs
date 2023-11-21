using Microsoft.AspNetCore.Components;

namespace RapidCMS.Core.Abstractions.Config;

public interface IHasNameDescription<TReturn>
{
    /// <summary>
    /// Sets the name of this field, used in table and list views.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    TReturn SetName(string name);

    /// <summary>
    /// Sets the description of this field, displayed under the name.
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    TReturn SetDescription(string description);

    /// <summary>
    /// Sets the details of this field, displayed under the control.
    /// </summary>
    /// <param name="details"></param>
    /// <returns></returns>
    TReturn SetDetails(MarkupString details);
}

namespace RapidCMS.Core.Abstractions.Config;

public interface IHasPlaceholder<TReturn>
{
    /// <summary>
    /// Sets the placeholder of this field, displayed in the editor.
    /// </summary>
    /// <param name="placeholder"></param>
    /// <returns></returns>
    TReturn SetPlaceholder(string placeholder);
}

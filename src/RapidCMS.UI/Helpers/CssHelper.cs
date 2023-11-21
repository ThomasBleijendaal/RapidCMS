using RapidCMS.Core.Enums;

namespace RapidCMS.UI.Helpers;

public static class CssHelper
{
    public static string GetValidationClass(ValidationState state)
    {
        return $"{(state.HasFlag(ValidationState.Valid) ? "is-valid " : "")}" +
            $"{(state.HasFlag(ValidationState.Invalid) ? "is-invalid " : "")}" +
            $"{(state.HasFlag(ValidationState.Modified) ? "is-modified " : "")}";
    }

    public static string GetMessageClass(MessageType type)
    {
        return "alert " + type switch
        {
            MessageType.Error => "alert-danger",
            MessageType.Information => "alert-info",
            MessageType.Success => "alert-success",
            MessageType.Warning => "alert-warning",
            _ => ""
        };
    }

    public static string GetDisplayModifier(ListType type)
    {
        return type switch
        {
            ListType.Block => "block",
            ListType.Table => "table",
            _ => ""
        };
    }
}

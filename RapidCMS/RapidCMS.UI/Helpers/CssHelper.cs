using RapidCMS.Common.Enums;

namespace RapidCMS.UI.Helpers
{
    public static class CssHelper
    {
        public static string GetValidationClass(ValidationState state)
        {
            return state == ValidationState.Valid
                ? "is-valid"
                : state == ValidationState.Invalid
                    ? "is-invalid"
                    : "";
        }
    }
}

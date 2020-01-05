using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Setup;

namespace RapidCMS.UI.Extensions
{
    public static class CustomRegistrationRenderFragmentExtensions
    {
        public static RenderFragment? ToRenderFragment(this ITypeRegistration? registration)
        {
            if (registration != null)
            {
                return builder =>
                {
                    builder.OpenComponent(0, registration.Type);
                    builder.CloseComponent();
                };
            }
            else
            {
                return default;
            }
        }
    }
}

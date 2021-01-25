using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RapidCMS.Example.Server.Pages
{
    public class SignInModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Content("~/")
            }, "OpenIdConnect");
        }
    }
}

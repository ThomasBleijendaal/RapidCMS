using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RapidCMS.Example.Server.Pages
{
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return SignOut(new AuthenticationProperties 
            {
                RedirectUri = Url.Content("~/") 
            }, "Cookies", "OpenIdConnect");
        }
    }
}

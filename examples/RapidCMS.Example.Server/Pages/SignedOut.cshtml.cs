using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RapidCMS.Example.Server.Pages
{
    public class SignedOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Redirect("/");
        }
    }
}

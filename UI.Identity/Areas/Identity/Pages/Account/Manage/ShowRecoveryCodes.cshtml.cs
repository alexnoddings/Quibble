using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quibble.UI.Identity.Areas.Identity.Pages.Account.Manage
{
    public class ShowRecoveryCodesModel : PageModel
    {
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "TempData must have a public getter and setter to function properly.")]
        [TempData]
        public IList<string>? RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;


        public IActionResult OnGet()
        {
            if (RecoveryCodes == null || RecoveryCodes.Count == 0)
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            return Page();
        }
    }
}

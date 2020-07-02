using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Quibble.Server.Models.Users;
using Quibble.Server.Services.SendGrid;

namespace Quibble.Server.Areas.Identity.Pages.Account
{
    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Username")]
        [StringLength(maximumLength: 26, MinimumLength = 4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(maximumLength: 128, MinimumLength = 12, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IAdvancedEmailSender _emailSender;

        public RegisterModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterModel> logger,
            IAdvancedEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public string? ReturnUrl { get; set; } = "/";

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public async Task OnGetAsync(string? returnUrl)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = await GetExternalAuthenticationSchemesAsync().ConfigureAwait(false);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "")
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = await GetExternalAuthenticationSchemesAsync().ConfigureAwait(false);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    string emailSubject = "Confirm your email";
                    string emailBody = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                    await _emailSender.SendEmailAsync(Input.Email, Input.UserName, emailSubject, emailBody).ConfigureAwait(false);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<List<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
        {
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false);
            return schemes.ToList();
        }
    }
}

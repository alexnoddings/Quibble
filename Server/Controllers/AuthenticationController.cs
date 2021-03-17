using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorIdentityBase.Server.Data;
using BlazorIdentityBase.Server.Services;
using BlazorIdentityBase.Shared.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorIdentityBase.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private static readonly string[] ExposedClaimTypes = {ClaimTypes.NameIdentifier, ClaimTypes.Name, ClaimTypes.Email};

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;

        private List<string> ModelStateErrors => 
            ModelState
                .SelectMany(kv => kv.Value?.Errors)
                .Select(modelError => modelError.ErrorMessage)
                .ToList();

        public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpGet("User")]
        public async Task<IActionResult> GetUserAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Ok(new UserInfo
                {
                    IsAuthenticated = true,
                    AuthenticationType = User.Identity.AuthenticationType ?? "Identity.Application",
                    UserName = User.Identity?.Name ?? string.Empty,
                    Claims = User.Claims.Where(claim => ExposedClaimTypes.Contains(claim.Type)).ToDictionary(claim => claim.Type, claim => claim.Value)
                });

            return Ok(UserInfo.Unauthenticated());
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest register)
        {
            if (register is null || !ModelState.IsValid)
                return BadRequest(ModelStateErrors);

            var user = new AppUser { UserName = register.UserName, Email = register.Email };
            var createUserResult = await _userManager.CreateAsync(user, register.Password);
            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelStateErrors);
            }

            return await LoginAsync(new LoginRequest { UserName = register.UserName, Password = register.Password, ShouldRememberUser = true });
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest login)
        {
            if (login is null || !ModelState.IsValid)
                return BadRequest(ModelStateErrors);

            var signInResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.ShouldRememberUser, false);
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("BadRequest", "Invalid username or password.");
                return BadRequest(ModelStateErrors);
            }

            var user = await _userManager.FindByNameAsync(login.UserName);
            var claims = await _userManager.GetClaimsAsync(user);
            var response = new UserInfo
            {
                IsAuthenticated = true,
                UserName = user.UserName,
                Claims = claims.ToDictionary(claim => claim.Type, claim => claim.Value)
            };
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
                await _signInManager.SignOutAsync();

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest forgotPassword)
        {
            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user is null)
                return Ok();

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string host = HttpContext.Request.Host.Value;

            var url = $"https://{host}/auth/resetPassword?email={user.Email}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendAsync(user.Email, url);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user is null)
            {
                ModelState.AddModelError("BadRequest", "Invalid email or token.");
                return BadRequest(ModelStateErrors);
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelStateErrors);
            }

            return await LoginAsync(new LoginRequest { UserName = user.UserName, Password = resetPassword.NewPassword });
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest changePassword)
        {
            var user = await _userManager.GetUserAsync(User);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelStateErrors);
            }

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [Authorize]
        [HttpPost("RequestChangeEmail")]
        public async Task<IActionResult> RequestChangeEmailAsync(RequestChangeEmailRequest requestChangeEmail)
        {
            var user = await _userManager.GetUserAsync(User);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, requestChangeEmail.Password);
            if (!isPasswordCorrect)
            {
                ModelState.AddModelError("BadPassword", "Incorrect password.");
                return BadRequest(ModelStateErrors);
            }

            var existingEmailUser = await _userManager.FindByEmailAsync(requestChangeEmail.NewEmail);
            if (existingEmailUser is not null)
            {
                ModelState.AddModelError("BadEmail", "Another user is registered with that email.");
                return BadRequest(ModelStateErrors);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, requestChangeEmail.NewEmail);
            string host = HttpContext.Request.Host.Value;

            var url = $"https://{host}/auth/changeEmail?email={requestChangeEmail.NewEmail}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendAsync(requestChangeEmail.NewEmail, url);
            return Ok();
        }

        [Authorize]
        [HttpPost("ChangeEmail")]
        public async Task<IActionResult> ChangeEmailAsync(ChangeEmailRequest changeEmail)
        {
            var user = await _userManager.GetUserAsync(User);

            var changeEmailResult = await _userManager.ChangeEmailAsync(user, changeEmail.NewEmail, changeEmail.Token);
            if (!changeEmailResult.Succeeded)
            {
                foreach (var error in changeEmailResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelStateErrors);
            }

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [Authorize]
        [HttpPost("ChangeUsername")]
        public async Task<IActionResult> ChangeUsernameAsync(ChangeUsernameRequest changeUsername)
        {
            var user = await _userManager.GetUserAsync(User);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, changeUsername.Password);
            if (!isPasswordCorrect)
            {
                ModelState.AddModelError("BadPassword", "Incorrect password.");
                return BadRequest(ModelStateErrors);
            }

            var existingUser = await _userManager.FindByNameAsync(changeUsername.NewUsername);
            if (existingUser is not null)
            {
                ModelState.AddModelError("BadUsername", "Another user is registered with that username.");
                return BadRequest(ModelStateErrors);
            }

            var changeEmailResult = await _userManager.SetUserNameAsync(user, changeUsername.NewUsername);
            if (!changeEmailResult.Succeeded)
            {
                foreach (var error in changeEmailResult.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelStateErrors);
            }

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }
    }
}

using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quibble.Shared.Api;

namespace Quibble.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        private string UserIdStr => User.FindFirstValue(ClaimTypes.NameIdentifier)
                                    ?? throw new InvalidOperationException("User is not authenticated.");
        public Guid UserId => Guid.Parse(UserIdStr);

        public ApiResponse ErrorResponse(string humanError) => new() {WasSuccessful = false, HumanError = humanError};
        public ApiResponse<TResult> ErrorResponse<TResult>(string humanError) => new() {WasSuccessful = false, HumanError = humanError};
        public ApiResponse<TResult> ErrorResponse<TResult>(string humanError, TResult result) => new() {WasSuccessful = false, HumanError = humanError, Result = result};

        public ApiResponse SuccessResponse() => new() {WasSuccessful = true};
        public ApiResponse<TResult> SuccessResponse<TResult>() => new() {WasSuccessful = true};
        public ApiResponse<TResult> SuccessResponse<TResult>(TResult result) => new() {WasSuccessful = true, Result = result};
    }
}

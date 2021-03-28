using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quibble.Server.FluentEnsure;

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

        [DebuggerStepThrough]
        public EnsureChained<TEntity> EnsureExists<TEntity>([NotNull] TEntity? entity)
        {
            if (entity is null)
                throw new ArgumentNullException();

            return new EnsureChained<TEntity>(entity, UserId);
        }

        [DebuggerStepThrough]
        public EnsureFirst<TEntity> Ensure<TEntity>(TEntity entity) => 
            new(entity, UserId);
    }
}

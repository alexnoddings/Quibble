using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quibble.Core.Entities;
using Quibble.Core.Extensions;
using Quibble.Host.Common;
using Quibble.Host.Common.Data;
using Quibble.Host.Common.Data.Entities;

namespace Quibble.Host.Hosted.Platform.Services
{
    public abstract class HostedServiceBase : IDisposable, IAsyncDisposable
    {
        private bool _disposed = false;

        protected IServiceProvider ServiceProvider { get; }
        protected HttpContext? HttpContext => ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        protected QuibbleDbContext DbContext { get; }

        protected HostedServiceBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            DbContext = ServiceProvider.GetRequiredService<QuibbleDbContext>();
        }

        protected async Task<DbQuibbleUser> GetCurrentUserAsync()
        {
            ClaimsPrincipal? user = HttpContext?.User;
            if (user?.Identity == null)
                throw ThrowHelper.Unauthenticated();

            DbQuibbleUser? dbUser = await DbContext.Users.FindAsync(user.GetId());
            if (dbUser == null)
                throw new InvalidOperationException($"Could not find user with id {user.GetId()}");

            return dbUser;
        }

        protected Task<Guid> GetOwnerIdAsync(IRound round)
        {
            if (DbContext == null) throw ThrowHelper.NullArgument(nameof(DbContext));

            var query =
                from quiz in DbContext.Quizzes
                where quiz.Id == round.QuizId
                select quiz.OwnerId;
            return query.FirstAsync();
        }

        protected Task<Guid> GetOwnerIdAsync(IQuestion question)
        {
            if (DbContext == null) throw ThrowHelper.NullArgument(nameof(DbContext));

            var query =
                from round in DbContext.Rounds
                where round.Id == question.RoundId
                join quiz in DbContext.Quizzes
                    on round.QuizId equals quiz.Id
                select quiz.OwnerId;
            return query.FirstAsync();
        }

        public ValueTask DisposeAsync() => DisposeAsync(true);

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                await DbContext.DisposeAsync();
            }

            Dispose(disposing);

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DbContext.Dispose();
            }

            _disposed = true;
        }
    }
}

using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quibble.Host.Common.Data;

namespace Quibble.Host.WASM.Server.Data
{
    /// <summary>
    /// Database abstraction for a combined <see cref="DbContext"/> using ASP.NET Identity and Identity Server.
    /// </summary>
    /// <seealso href="https://github.com/dotnet/aspnetcore/blob/4abf766c3c07bdafc3918a10fddbdc3ec8fd086b/src/Identity/ApiAuthorization.IdentityServer/src/Data/ApiAuthorizationDbContext.cs"/>
    public class QuibbleClientSideDbContext : QuibbleDbContext, IPersistedGrantDbContext
    {
        private IOptions<OperationalStoreOptions> OperationalStoreOptions { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="QuibbleClientSideDbContext"/>.
        /// </summary>
        /// <param name="options">The <see cref="DbContextOptions"/>.</param>
        /// <param name="operationalStoreOptions">The <see cref="IOptions{OperationalStoreOptions}"/>.</param>
        public QuibbleClientSideDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options)
        {
            OperationalStoreOptions = operationalStoreOptions ?? throw new ArgumentNullException(nameof(operationalStoreOptions));
        }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{PersistedGrant}"/>.
        /// </summary>
        public DbSet<PersistedGrant> PersistedGrants { get; set; } = default!;

        /// <summary>
        /// Gets or sets the <see cref="DbSet{DeviceFlowCodes}"/>.
        /// </summary>
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; } = default!;

        /// <inheritdoc />
        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(OperationalStoreOptions.Value);
        }
    }
}
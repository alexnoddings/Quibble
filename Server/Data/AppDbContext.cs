using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Data.Models;

namespace Quibble.Server.Data;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<DbQuiz> Quizzes { get; set; } = default!;
    public DbSet<DbRound> Rounds { get; set; } = default!;
    public DbSet<DbQuestion> Questions { get; set; } = default!;
    public DbSet<DbSubmittedAnswer> SubmittedAnswers { get; set; } = default!;
    public DbSet<DbParticipant> Participants { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

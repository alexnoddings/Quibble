using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quibble.Server.Core.Models;
using System.Reflection;

namespace Quibble.Server.Core;

public class AppDbContext : IdentityDbContext<DbUser, DbRole, Guid>
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

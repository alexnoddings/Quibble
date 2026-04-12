using Microsoft.EntityFrameworkCore;
using Quibble.Data.Entites.Answers;
using Quibble.Data.Entites.Games;
using Quibble.Data.Entites.Questions;
using Quibble.Data.Entites.Rounds;
using Quibble.Data.Entites.Users;

namespace Quibble.Data;

public class QuibbleDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Participant> Participants => Set<Participant>();

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<Question> Questions => Set<Question>();

    public DbSet<SubmittedAnswer> Answers => Set<SubmittedAnswer>();

    public QuibbleDbContext(DbContextOptions<QuibbleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var thisAssembly = typeof(QuibbleDbContext).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(thisAssembly);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quibble.Data.Entites.Games;

namespace Quibble.Data.Entites.Users;

public class User : Entity
{
    public Guid ObjectId { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public List<Game> OwnedGames { get; init; } = null!;
    public List<Participant> Participations { get; init; } = null!;
}

public static class UserConstraints
{
    public static class DisplayName
    {
        public const int MaxLength = 20;
    }
}

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.IsEntity();

        builder
            .HasIndex(u => u.ObjectId)
            .IsUnique();

        builder
            .Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(UserConstraints.DisplayName.MaxLength);
    }
}

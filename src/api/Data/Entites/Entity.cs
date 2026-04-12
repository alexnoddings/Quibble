using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Quibble.Data.Entites;

public abstract class Entity
{
    public Guid Id { get; set; }
}

public static class EntityTypeConfigurationExtensions
{
    public static void IsEntity<T>(this EntityTypeBuilder<T> builder) where T : Entity =>
        builder.HasKey(x => x.Id);
}

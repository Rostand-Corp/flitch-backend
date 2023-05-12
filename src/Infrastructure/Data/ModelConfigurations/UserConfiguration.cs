using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ModelConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property<string>(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property<string>(u => u.FullName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property<string>(u => u.Status)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.HasOne<Color>(u => u.Color)
            .WithMany()
            .HasForeignKey(u => u.ColorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany<Chat>(u=>u.Chats)
            .WithMany(c => c.Users)
            .UsingEntity<ChatUser>(
                j => j.Property(e => e.Joined).HasDefaultValueSql("CURRENT_TIMESTAMP"));
    }
}
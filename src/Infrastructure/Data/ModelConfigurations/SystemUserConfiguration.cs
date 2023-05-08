using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ModelConfigurations;

public class SystemUserConfiguration : IEntityTypeConfiguration<SystemUser>
{
    public void Configure(EntityTypeBuilder<SystemUser> builder)
    {
        builder.HasOne<User>(su => su.MessengerUser)
            .WithOne()
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property<string>(su => su.FullName)
            .IsRequired()
            .HasMaxLength(128);
    }
}
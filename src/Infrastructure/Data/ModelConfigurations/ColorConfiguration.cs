using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ModelConfigurations;

public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.ToTable("Colors");

        builder.HasKey(c => c.Id);

        builder.Property<string>(c => c.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property<string>(c => c.HexCode)
            .IsRequired()
            .HasMaxLength(6);


    }
}
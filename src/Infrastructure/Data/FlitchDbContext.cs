using Domain.Entities;
using Infrastructure.Data.ModelConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class FlitchDbContext : IdentityDbContext<IdentityUser>
{
    public FlitchDbContext(DbContextOptions<FlitchDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Better to be called first to ensure Identity stores are all good

        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new ColorConfiguration());
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Color> Colors { get; set; } // Will be added implicitly anyway

}

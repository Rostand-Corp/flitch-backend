using Domain.Entities;
using Infrastructure.Data.ModelConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class FlitchDbContext : IdentityDbContext<SystemUser, IdentityRole<Guid>, Guid>
{
    public FlitchDbContext(DbContextOptions<FlitchDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Better to be called first to ensure Identity stores are all good

        builder.ApplyConfiguration(new SystemUserConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new ColorConfiguration());
        builder.ApplyConfiguration(new ChatConfiguration());
        builder.ApplyConfiguration(new MessageConfiguration());
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Color> Colors { get; set; } // Will be added implicitly anyway
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }

}

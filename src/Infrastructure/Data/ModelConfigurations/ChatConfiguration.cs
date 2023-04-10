using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ModelConfigurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder
            .ToTable("Chats");

        builder
            .HasKey(c => c.Id);

        builder
            .Property<string?>(c => c.Name)
            .HasMaxLength(50)
            .IsRequired(false);

        builder
            .Property<ChatType>(c => c.Type)
            .IsRequired(true);


        builder
            .HasMany<Message>(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId)
            .IsRequired();

        builder
            .HasOne<Message>(c => c.LastMessage)
            .WithOne()
            .HasForeignKey<Chat>(c => c.LastMessageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Property<DateTime>(c => c.Created)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

    }
}
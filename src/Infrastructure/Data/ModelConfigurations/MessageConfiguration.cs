using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.ModelConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder
            .ToTable("Messages");

        builder
            .HasKey(m => m.Id);

        builder
            .Property<string>(m => m.Content)
            .HasMaxLength(500)
            .IsRequired();

        // public MessageType Type { get; set; }
        builder
            .Property<MessageType>(m => m.Type)
            .HasDefaultValue(MessageType.Default);

        builder
            .Property<DateTime>(m => m.Timestamp)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property<bool>(m => m.IsVisible)
            .HasDefaultValue(true);

        builder
            .HasOne<ChatUser>(m => m.Sender)
            .WithMany(cu => cu.Messages)
            .HasForeignKey(m => m.SenderId)
            .IsRequired();

        // public Message? ReplyTo { get; set; }
        builder
            .HasOne<Message>(m => m.ReplyTo)
            .WithOne();

        // public Guid ChatId { get; set; }
        // public Chat Chat { get; set; }
            builder
            .HasOne<Chat>(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId);
        
        
    }
}
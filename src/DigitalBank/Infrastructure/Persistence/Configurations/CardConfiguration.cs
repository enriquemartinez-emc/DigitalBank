using DigitalBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalBank.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.AccountId).IsRequired();
        builder.Property(c => c.CardType).IsRequired();
        builder.Property(c => c.CardHolderName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.CardNumber).IsRequired().HasMaxLength(16);
        builder.Property(c => c.ExpiryDate).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();

        builder.HasOne<Account>().WithMany(a => a.Cards).HasForeignKey(c => c.AccountId);
        builder.HasIndex(c => c.CardNumber).IsUnique();
    }
}

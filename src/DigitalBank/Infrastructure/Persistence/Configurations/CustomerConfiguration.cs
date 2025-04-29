using DigitalBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalBank.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);
    }
}

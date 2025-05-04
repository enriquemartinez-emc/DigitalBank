using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank.Infrastructure.Persistence;

public class DigitalBankDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<User> Users { get; set; }

    public DigitalBankDbContext(DbContextOptions<DigitalBankDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DigitalBankDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

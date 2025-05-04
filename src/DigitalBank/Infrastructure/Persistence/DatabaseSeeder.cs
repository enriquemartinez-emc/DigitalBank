using DigitalBank.Domain.Common;
using DigitalBank.Domain.Entities;

namespace DigitalBank.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(DigitalBankDbContext dbContext)
    {
        // Check if the database is empty (no customers)
        if (!dbContext.Customers.Any())
        {
            // Seed a default customer using the Create method
            var customerResult = Customer.Create("John", "Doe", "john.doe@example.com");
            if (customerResult.IsSuccess)
            {
                var customer = customerResult.Value!;
                dbContext.Customers.Add(customer);

                // Seed a default account for the customer
                var accountResult = Account.Create("12345678901234", customer.Id, 1000m);
                if (accountResult.IsSuccess)
                {
                    var account = accountResult.Value!;
                    dbContext.Accounts.Add(account);

                    var cardResult = Card.Create(
                        account.Id,
                        CardType.Debit,
                        "John Doe",
                        "John Doe",
                        0);
                    if (cardResult.IsSuccess)
                    {
                        dbContext.Cards.Add(cardResult.Value!);
                    }
                }

                // Save changes to the database
                dbContext.SaveChanges();
            }
        }

        if (!dbContext.Users.Any())
        {
            var defaultUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Email = "admin@digitalbank.com"
            };
            dbContext.Users.Add(defaultUser);
            dbContext.SaveChanges();
        }
    }
}

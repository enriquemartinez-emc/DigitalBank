using DigitalBank.Domain.Entities;

namespace DigitalBank.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(DigitalBankDbContext context)
    {
        // Check if the database is empty (no customers)
        if (!context.Customers.Any())
        {
            // Seed a default customer
            var customer = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            context.Customers.Add(customer);

            // Seed a default account for the customer
            var accountResult = Account.Create("12345678901234", customer.Id, 1000m);
            if (accountResult.IsSuccess)
            {
                context.Accounts.Add(accountResult.Value!);
            }

            // Save changes to the database
            context.SaveChanges();
        }
    }
}
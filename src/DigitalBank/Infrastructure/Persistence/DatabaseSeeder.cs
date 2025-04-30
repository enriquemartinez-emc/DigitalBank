using DigitalBank.Domain.Entities;

namespace DigitalBank.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(DigitalBankDbContext context)
    {
        // Check if the database is empty (no customers)
        if (!context.Customers.Any())
        {
            // Seed a default customer using the Create method
            var customerResult = Customer.Create("John", "Doe", "john.doe@example.com");
            if (customerResult.IsSuccess)
            {
                var customer = customerResult.Value!;
                context.Customers.Add(customer);

                // Seed a default account for the customer
                var accountResult = Account.Create("12345678901234", customer.Id, 1000m);
                if (accountResult.IsSuccess)
                {
                    var account = accountResult.Value!;
                    context.Accounts.Add(account);

                    var cardResult = Card.Create(
                        account.Id,
                        CardType.Debit,
                        "John Doe",
                        "John Doe",
                        0);
                    if (cardResult.IsSuccess)
                    {
                        context.Cards.Add(cardResult.Value!);
                    }
                }

                // Save changes to the database
                context.SaveChanges();
            }
        }
    }
}

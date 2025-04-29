namespace DigitalBank.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public List<Account> Accounts { get; private set; } = [];

    public Customer(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}

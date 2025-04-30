using DigitalBank.Domain.Common;
using System.Text.RegularExpressions;

namespace DigitalBank.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public List<Account> Accounts { get; private set; } = [];

    private Customer() { } // For EF Core

    public static Result<Customer> Create(
        string firstName,
        string lastName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > 50 || !Regex.IsMatch(firstName, @"^[a-zA-Z\s-]+$"))
            return Result.Failure<Customer>(Errors.Customer.InvalidFirstName);

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > 50 || !Regex.IsMatch(lastName, @"^[a-zA-Z\s-]+$"))
            return Result.Failure<Customer>(Errors.Customer.InvalidLastName);

        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return Result.Failure<Customer>(Errors.Customer.InvalidEmail);

        return Result.Success(new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLower()
        });
    }
}

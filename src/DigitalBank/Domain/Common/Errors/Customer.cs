namespace DigitalBank.Domain.Common.Errors;

public static partial class Errors
{
    public static class Customer
    {
        public static Error InvalidFirstName => new("Customer.InvalidFirstName", "First name must be 1-50 characters and contain only letters, spaces, or hyphens.");
        public static Error InvalidLastName => new("Customer.InvalidLastName", "Last name must be 1-50 characters and contain only letters, spaces, or hyphens.");
        public static Error InvalidEmail => new("Customer.InvalidEmail", "Email must be a valid email address.");
        public static Error DuplicateEmail => new("Customer.DuplicateEmail", "Email is already in use.");
        public static Error NotFound => new("Customer.NotFound", "Customer not found.");
    }
}

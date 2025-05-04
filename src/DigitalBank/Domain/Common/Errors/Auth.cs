namespace DigitalBank.Domain.Common.Errors;

public static partial class Errors
{
    public static class Auth
    {
        public static Error InvalidCredentials => new("Auth.InvalidCredentials", "Invalid username or password");
    }
}


namespace DigitalBank.Domain.Common.Errors;

public static partial class Errors
{
    public static class Transfer
    {
        public static Error NotFound => new("Transfer.NotFound", "No transfers found for the specified customer.");
    }
}
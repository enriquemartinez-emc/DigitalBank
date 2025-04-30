namespace DigitalBank.Domain.Common.Errors;

public static partial class Errors
{
    public static class Card
    {
        public static Error InvalidAccount => new("Card.InvalidAccount", "Account ID is invalid.");
        public static Error TooManyCards => new("Card.TooManyCards", "Account cannot have more than 3 cards.");
        public static Error InvalidCardHolderName => new("Card.InvalidCardHolderName", "Card holder name must match customer's full name.");
        public static Error NotFound => new("Card.NotFound", "Card not found.");
    }
}

namespace DigitalBank.Domain.Events;

public record AccountBalanceUpdated(Guid AccountId, decimal NewBalance);

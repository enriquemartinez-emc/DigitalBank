namespace DigitalBank.Application.Features.Transfers;

public record TransferResponse
(
    Guid Id,
    decimal Amount,
    string FromAccountNumber,
    string ToAccountNumber,
    string FromCustomerFullName,
    string ToCustomerFullName,
    DateTime CreatedAt
);
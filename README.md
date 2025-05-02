# DigitalBank

A .NET 8 digital banking application using Vertical Slice Architecture, DDD, CQRS, and Entity Framework Core with PostgreSQL.

## Setup

1. Install .NET 8 SDK
2. Install Docker (for integration tests and deployment)
3. Update connection string in `appsettings.json`
4. Run migrations: `dotnet ef migrations add InitialCreate`
5. Run the application: `dotnet run --project src/DigitalBank.Api`

## Running Tests

- **Unit Tests**: `cd src/DigitalBank.Tests && dotnet test`
- **Integration Tests**: Ensure Docker is running, then run `cd src/DigitalBank.Tests && dotnet test`. Requires Testcontainers to spin up PostgreSQL containers.

## Features

- Account management (create, get balance)
- Customer management (create, get)
- Card management (issue, get)
- Money transfers
- Input validation
- Global error handling
- CQRS implementation
- Database seeding
- Integration testing with Testcontainers

## API Endpoints

- **POST /api/customers**: Create a customer
- **GET /api/customers/{customerId}**: Get a customer by ID
- **POST /api/cards**: Issue a card for an account
- **GET /api/cards/{cardId}**: Get a card by ID
  **GET /api/customers/{customerId}/accounts**: Get accounts by customer
- **POST /api/customers/{customerId}/accounts**: Create an account
- **GET /api/customers/{customerId}/accounts/{accountId}/balance**: Get account balance
- **POST /api/transfers**: Create a transfer
- **GET /health**: Check API health

## TODO:

- CI/CD with GitHub Actions

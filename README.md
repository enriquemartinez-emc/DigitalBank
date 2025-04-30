# DigitalBank
A .NET 8 digital banking application using Vertical Slice Architecture, DDD, CQRS, and Entity Framework Core with PostgreSQL.

## Setup
1. Install .NET 8 SDK
2. Install PostgreSQL
3. Install Docker (for integration tests)
4. Update connection string in `appsettings.json`
5. Run migrations: `dotnet ef migrations add InitialCreate`
6. Run the application: `dotnet run --project src/DigitalBank.Api`

## Running Tests
- **Unit Tests**: `cd src/DigitalBank.Tests && dotnet test`
- **Integration Tests**: Ensure Docker is running, then run `cd src/DigitalBank.Tests && dotnet test`. Requires Testcontainers to spin up PostgreSQL containers.

## Features
- Account management
- Balance inquiries
- Money transfers
- Input validation
- Global error handling
- CQRS implementation
- Database seeding
- Integration testing with Testcontainers
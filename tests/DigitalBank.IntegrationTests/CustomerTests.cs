using DigitalBank.Application.Features.Customers;
using DigitalBank.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalBank.IntegrationTests;

public class CustomerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateCustomer_ValidInput_CreatesCustomerAndReturnsId()
    {
        // Arrange
        var command = new CreateCustomerCommand(
            FirstName: "Jane",
            LastName: "Doe",
            Email: "jane.doe@example.com");

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        using var dbContext = await CreateDbContextAsync();
        var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == result.Value);
        Assert.NotNull(customer);
        Assert.Equal(command.FirstName, customer.FirstName);
        Assert.Equal(command.LastName, customer.LastName);
        Assert.Equal(command.Email.ToLower(), customer.Email);
    }

    [Fact]
    public async Task CreateCustomer_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();
        }

        var command = new CreateCustomerCommand("Jane", "Doe", "john.doe@example.com");

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Customer.DuplicateEmail", result.Error!.Code);
    }

    [Fact]
    public async Task GetCustomer_ValidId_ReturnsCustomer()
    {
        // Arrange
        var customer = Customer.Create("John", "Doe", "john.doe@example.com").Value!;
        using (var dbContext = await CreateDbContextAsync())
        {
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();
        }

        var query = new GetCustomerQuery(customer.Id);

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(customer.Id, result.Value!.Id);
        Assert.Equal(customer.FirstName, result.Value.FirstName);
    }

    [Fact]
    public async Task GetCustomer_InvalidId_ReturnsFailure()
    {
        // Arrange
        var query = new GetCustomerQuery(Guid.NewGuid());

        // Act
        using var scope = ServiceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(query);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Customer.NotFound", result.Error!.Code);
    }
}

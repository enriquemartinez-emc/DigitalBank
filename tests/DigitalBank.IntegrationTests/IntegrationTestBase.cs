using DigitalBank.Application.Common;
using DigitalBank.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace DigitalBank.IntegrationTests;

public class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    protected IServiceProvider ServiceProvider { get; }

    public IntegrationTestBase()
    {
        // Set up PostgreSQL container  
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("digitalbank")
            .WithUsername("digitalbankuser")
            .WithPassword("digitalbankpassword")
            .Build();

        // Set up DI container  
        var services = new ServiceCollection();
        services.AddDbContext<DigitalBankDbContext>(options =>
            options.UseNpgsql(_postgreSqlContainer.GetConnectionString()));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IntegrationTestBase).Assembly));
        services.AddValidatorsFromAssembly(typeof(IntegrationTestBase).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        // Start the PostgreSQL container  
        await _postgreSqlContainer.StartAsync();

        // Apply migrations to create the database schema  
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DigitalBankDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // Stop and dispose of the PostgreSQL container  
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }

    protected async Task<DigitalBankDbContext> CreateDbContextAsync()
    {
        var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DigitalBankDbContext>();
        await dbContext.Database.CanConnectAsync();
        return dbContext;
    }
}

using DigitalBank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalBank;

public static class ConfigureApplication
{
    public static void Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowFrontend");
        // app.UseHttpsRedirection();
        // app.UseAuthorization();
        app.MapControllers();
        app.EnsureDatabaseCreated();
    }

    private static void EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DigitalBankDbContext>();
        db.Database.Migrate();
        if (app.Environment.IsDevelopment())
        {
            DatabaseSeeder.Seed(db);
        }
    }
}

using Conexa.Infrastructure.Persistence;

namespace Conexa.Api;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
        await initializer.SeedAsync();
    }

    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}

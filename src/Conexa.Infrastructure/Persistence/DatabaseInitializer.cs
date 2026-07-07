using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using Conexa.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Conexa.Infrastructure.Persistence;

public class DatabaseInitializer(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    IOptions<SeedOptions> seedOptions,
    ILogger<DatabaseInitializer> logger)
{
    private readonly SeedOptions _seed = seedOptions.Value;

    public Task InitializeAsync() => context.Database.MigrateAsync();

    public async Task SeedAsync()
    {
        if (!_seed.Enabled)
        {
            logger.LogInformation("Database seeding is disabled; skipping.");
            return;
        }

        await EnsureUserAsync(_seed.AdminEmail, _seed.AdminPassword, UserRole.Admin);
        await EnsureUserAsync(_seed.RegularEmail, _seed.RegularPassword, UserRole.Regular);

        await context.SaveChangesAsync();
    }

    private async Task EnsureUserAsync(string? email, string? password, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Seed credentials for role {Role} are missing; skipping that user.", role);
            return;
        }

        var normalizedEmail = User.NormalizeEmail(email);

        if (await context.Users.AnyAsync(u => u.Email == normalizedEmail))
            return;

        context.Users.Add(User.Create(normalizedEmail, passwordHasher.Hash(password), role));
    }
}

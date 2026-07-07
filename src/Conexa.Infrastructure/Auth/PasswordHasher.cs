using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Conexa.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(user: null!, password);

    public bool Verify(string passwordHash, string providedPassword) =>
        _hasher.VerifyHashedPassword(user: null!, passwordHash, providedPassword)
            is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
}

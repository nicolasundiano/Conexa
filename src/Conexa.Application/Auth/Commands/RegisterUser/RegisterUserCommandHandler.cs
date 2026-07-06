using Conexa.Application.Auth.Common;
using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var email = User.NormalizeEmail(command.Email);

        var emailTaken = await context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailTaken)
            throw new ConflictException($"A user with email '{email}' already exists.");

        var user = User.Register(email, passwordHasher.Hash(command.Password));

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        var token = tokenService.CreateToken(user);

        return new AuthResponse(token.AccessToken, token.ExpiresAtUtc, user.Email, user.Role.ToString());
    }
}

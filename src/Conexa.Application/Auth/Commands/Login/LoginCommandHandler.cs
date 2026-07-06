using Conexa.Application.Auth.Common;
using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Auth.Commands.Login;

public class LoginCommandHandler(
    IApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = User.NormalizeEmail(command.Email);

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
        {
            passwordHasher.Hash(command.Password);
            throw new InvalidCredentialsException();
        }

        if (!passwordHasher.Verify(user.PasswordHash, command.Password))
            throw new InvalidCredentialsException();

        var token = tokenService.CreateToken(user);

        return new AuthResponse(token.AccessToken, token.ExpiresAtUtc, user.Email, user.Role.ToString());
    }
}

using Conexa.Application.Auth.Commands.RegisterUser;
using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using Conexa.Domain.Enums;
using MockQueryable.NSubstitute;
using NSubstitute;
using Xunit;

namespace Conexa.UnitTests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();

    private RegisterUserCommandHandler CreateHandler(params User[] existingUsers)
    {
        var usersDbSet = existingUsers.BuildMockDbSet();
        _context.Users.Returns(usersDbSet);
        return new RegisterUserCommandHandler(_context, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_WithNewEmail_CreatesUserAndReturnsToken()
    {
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed");
        _tokenService.CreateToken(Arg.Any<User>())
            .Returns(new TokenResult("token", DateTime.UtcNow.AddHours(1)));
        var handler = CreateHandler();

        var response = await handler.Handle(new RegisterUserCommand("  New@Conexa.com ", "Passw0rd"), default);

        Assert.Equal("token", response.AccessToken);
        _context.Users.Received(1).Add(Arg.Is<User>(u =>
            u.Email == "new@conexa.com" && u.Role == UserRole.Regular && u.PasswordHash == "hashed"));
        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ThrowsConflict()
    {
        var existing = User.Register("taken@conexa.com", "hash");
        var handler = CreateHandler(existing);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new RegisterUserCommand("taken@conexa.com", "Passw0rd"), default));
    }
}

using Conexa.Application.Auth.Commands.Login;
using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using MockQueryable.NSubstitute;
using NSubstitute;
using Xunit;

namespace Conexa.UnitTests.Application;

public class LoginCommandHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();

    private LoginCommandHandler CreateHandler(params User[] existingUsers)
    {
        var usersDbSet = existingUsers.BuildMockDbSet();
        _context.Users.Returns(usersDbSet);
        return new LoginCommandHandler(_context, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        var user = User.Register("user@conexa.com", "storedHash");
        _passwordHasher.Verify("storedHash", "Passw0rd").Returns(true);
        _tokenService.CreateToken(Arg.Any<User>())
            .Returns(new TokenResult("token", DateTime.UtcNow.AddHours(1)));
        var handler = CreateHandler(user);

        var response = await handler.Handle(new LoginCommand("user@conexa.com", "Passw0rd"), default);

        Assert.Equal("token", response.AccessToken);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsInvalidCredentials()
    {
        var user = User.Register("user@conexa.com", "storedHash");
        _passwordHasher.Verify("storedHash", Arg.Any<string>()).Returns(false);
        var handler = CreateHandler(user);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            handler.Handle(new LoginCommand("user@conexa.com", "wrong"), default));
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ThrowsInvalidCredentials()
    {
        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            handler.Handle(new LoginCommand("unknown@conexa.com", "Passw0rd"), default));
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_RunsDummyHashToEqualizeTiming()
    {
        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            handler.Handle(new LoginCommand("unknown@conexa.com", "Passw0rd"), default));

        _passwordHasher.Received(1).Hash("Passw0rd");
    }
}

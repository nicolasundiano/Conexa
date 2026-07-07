using Conexa.Domain.Common;
using Conexa.Domain.Entities;
using Conexa.Domain.Enums;
using Xunit;

namespace Conexa.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Register_CreatesRegularUser()
    {
        var user = User.Register("test@conexa.com", "hash");

        Assert.Equal(UserRole.Regular, user.Role);
        Assert.Equal("test@conexa.com", user.Email);
    }

    [Fact]
    public void Create_WithAdminRole_CreatesAdmin()
    {
        var user = User.Create("admin@conexa.com", "hash", UserRole.Admin);

        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void Create_NormalizesEmail()
    {
        var user = User.Register("  Test@Conexa.COM  ", "hash");

        Assert.Equal("test@conexa.com", user.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyEmail_Throws(string email)
    {
        Assert.Throws<DomainException>(() => User.Register(email, "hash"));
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_Throws()
    {
        Assert.Throws<DomainException>(() => User.Register("test@conexa.com", ""));
    }

    [Theory]
    [InlineData("  Test@Conexa.COM  ", "test@conexa.com")]
    [InlineData("USER@X.IO", "user@x.io")]
    public void NormalizeEmail_TrimsAndLowercases(string input, string expected)
    {
        Assert.Equal(expected, User.NormalizeEmail(input));
    }
}

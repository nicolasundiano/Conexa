using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Conexa.IntegrationTests;

public class AuthEndpointsTests(ConexaApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task Register_WithNewEmail_ReturnsToken()
    {
        var response = await CreateClient().PostAsJsonAsync("/api/auth/register",
            new { email = "new-user@conexa.test", password = "Passw0rd" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TokenBody>();
        Assert.False(string.IsNullOrWhiteSpace(body!.AccessToken));
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsConflict()
    {
        var response = await CreateClient().PostAsJsonAsync("/api/auth/register",
            new { email = AdminEmail, password = "Passw0rd" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        var response = await CreateClient().PostAsJsonAsync("/api/auth/register",
            new { email = "weak@conexa.test", password = "123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var response = await CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = AdminEmail, password = AdminPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var response = await CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = AdminEmail, password = "wrong-password" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private sealed record TokenBody(string AccessToken);
}

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Conexa.Application.Auth.Common;
using Xunit;

namespace Conexa.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public abstract class TestBase(ConexaApiFactory factory)
{
    protected const string AdminEmail = "admin@conexa.test";
    protected const string AdminPassword = "Admin123!";
    protected const string RegularEmail = "user@conexa.test";
    protected const string RegularPassword = "User123!";

    protected readonly ConexaApiFactory Factory = factory;

    protected HttpClient CreateClient() => Factory.CreateClient();

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = CreateClient();
        var token = await GetTokenAsync(client, email, password);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected Task<HttpClient> CreateAdminClientAsync() => CreateAuthenticatedClientAsync(AdminEmail, AdminPassword);
    protected Task<HttpClient> CreateRegularClientAsync() => CreateAuthenticatedClientAsync(RegularEmail, RegularPassword);

    private static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return body!.AccessToken;
    }
}

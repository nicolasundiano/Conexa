using System.Net;
using System.Net.Http.Json;
using Conexa.Application.Movies.Common;
using Xunit;

namespace Conexa.IntegrationTests;

public class MoviesAuthorizationTests(ConexaApiFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task GetMovies_WithoutToken_ReturnsUnauthorized()
    {
        var response = await CreateClient().GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMovies_WithAnyAuthenticatedUser_ReturnsOk()
    {
        var client = await CreateRegularClientAsync();

        var response = await client.GetAsync("/api/movies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMovieById_AsAdmin_ReturnsForbidden()
    {
        var admin = await CreateAdminClientAsync();
        var id = await CreateMovieAsAdminAsync(admin);

        var response = await admin.GetAsync($"/api/movies/{id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMovieById_AsRegular_ReturnsOk()
    {
        var admin = await CreateAdminClientAsync();
        var id = await CreateMovieAsAdminAsync(admin);
        var regular = await CreateRegularClientAsync();

        var response = await regular.GetAsync($"/api/movies/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateMovie_AsRegular_ReturnsForbidden()
    {
        var regular = await CreateRegularClientAsync();

        var response = await regular.PostAsJsonAsync("/api/movies", new { title = "Test" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateMovie_AsAdmin_ReturnsCreated()
    {
        var admin = await CreateAdminClientAsync();

        var response = await admin.PostAsJsonAsync("/api/movies", new { title = "The Great Movie" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SyncMovies_AsRegular_ReturnsForbidden()
    {
        var regular = await CreateRegularClientAsync();

        var response = await regular.PostAsync("/api/movies/sync", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static async Task<int> CreateMovieAsAdminAsync(HttpClient admin)
    {
        var response = await admin.PostAsJsonAsync("/api/movies", new { title = "Seeded Movie", episodeId = 4 });
        response.EnsureSuccessStatusCode();
        var movie = await response.Content.ReadFromJsonAsync<MovieDto>();
        return movie!.Id;
    }
}

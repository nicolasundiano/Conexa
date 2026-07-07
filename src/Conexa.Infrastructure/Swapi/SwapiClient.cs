using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Polly;

namespace Conexa.Infrastructure.Swapi;

public class SwapiClient(HttpClient httpClient) : ISwapiClient
{
    public async Task<IReadOnlyList<SwapiFilm>> GetFilmsAsync(CancellationToken cancellationToken = default)
    {
        SwapiFilmsResponse? response;

        try
        {
            response = await httpClient.GetFromJsonAsync<SwapiFilmsResponse>("films", cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
                                       or JsonException or ExecutionRejectedException)
        {
            throw new ExternalServiceException("Failed to fetch films from the Star Wars API.", ex);
        }

        if (response?.Result is null)
            throw new ExternalServiceException("The Star Wars API returned an unexpected response.");

        return response.Result
            .Where(item => item.Properties is not null)
            .Select(item => Map(item.Properties!))
            .ToList();
    }

    private static SwapiFilm Map(SwapiFilmProperties p) => new(
        p.Title,
        p.EpisodeId,
        p.OpeningCrawl,
        p.Director,
        p.Producer,
        DateOnly.TryParse(p.ReleaseDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null,
        p.Url);

    private sealed record SwapiFilmsResponse(
        [property: JsonPropertyName("result")] List<SwapiFilmResult> Result);

    private sealed record SwapiFilmResult(
        [property: JsonPropertyName("properties")] SwapiFilmProperties? Properties);

    private sealed record SwapiFilmProperties(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("episode_id")] int? EpisodeId,
        [property: JsonPropertyName("opening_crawl")] string? OpeningCrawl,
        [property: JsonPropertyName("director")] string? Director,
        [property: JsonPropertyName("producer")] string? Producer,
        [property: JsonPropertyName("release_date")] string? ReleaseDate,
        [property: JsonPropertyName("url")] string? Url);
}

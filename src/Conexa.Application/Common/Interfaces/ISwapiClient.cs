using Conexa.Application.Movies.Common;

namespace Conexa.Application.Common.Interfaces;

public record SwapiFilm(
    string Title,
    int? EpisodeId,
    string? OpeningCrawl,
    string? Director,
    string? Producer,
    DateOnly? ReleaseDate,
    string Url) : IMovieDetailsSource;

public interface ISwapiClient
{
    Task<IReadOnlyList<SwapiFilm>> GetFilmsAsync(CancellationToken cancellationToken = default);
}

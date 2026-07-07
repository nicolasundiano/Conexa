using Conexa.Domain.Entities;

namespace Conexa.Application.Movies.Common;

public record MovieDto(
    int Id,
    string Title,
    int? EpisodeId,
    string? OpeningCrawl,
    string? Director,
    string? Producer,
    DateOnly? ReleaseDate,
    string? ExternalUrl,
    DateTime CreatedAt,
    DateTime? LastModifiedAt)
{
    public static MovieDto From(Movie movie) => new(
        movie.Id,
        movie.Title,
        movie.EpisodeId,
        movie.OpeningCrawl,
        movie.Director,
        movie.Producer,
        movie.ReleaseDate,
        movie.ExternalUrl,
        movie.CreatedAt,
        movie.LastModifiedAt);
}

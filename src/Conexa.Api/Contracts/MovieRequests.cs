namespace Conexa.Api.Contracts;

public record CreateMovieRequest(
    string Title,
    int? EpisodeId,
    string? OpeningCrawl,
    string? Director,
    string? Producer,
    DateOnly? ReleaseDate);

public record UpdateMovieRequest(
    string Title,
    int? EpisodeId,
    string? OpeningCrawl,
    string? Director,
    string? Producer,
    DateOnly? ReleaseDate);

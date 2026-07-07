namespace Conexa.Application.Movies.Common;

public record MovieListItemDto(
    int Id,
    string Title,
    int? EpisodeId,
    string? Director,
    DateOnly? ReleaseDate);

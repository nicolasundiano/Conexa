namespace Conexa.Application.Movies.Common;

public interface IMovieDetailsSource
{
    string Title { get; }
    int? EpisodeId { get; }
    string? OpeningCrawl { get; }
    string? Director { get; }
    string? Producer { get; }
    DateOnly? ReleaseDate { get; }
}

using Conexa.Domain.ValueObjects;

namespace Conexa.Application.Movies.Common;

public static class MovieDetailsSourceExtensions
{
    public static MovieDetails ToDetails(this IMovieDetailsSource source) => new(
        source.Title,
        source.EpisodeId,
        source.OpeningCrawl,
        source.Director,
        source.Producer,
        source.ReleaseDate);
}

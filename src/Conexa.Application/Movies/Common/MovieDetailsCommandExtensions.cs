using Conexa.Domain.ValueObjects;

namespace Conexa.Application.Movies.Common;

public static class MovieDetailsCommandExtensions
{
    public static MovieDetails ToDetails(this IMovieDetailsCommand command) => new(
        command.Title,
        command.EpisodeId,
        command.OpeningCrawl,
        command.Director,
        command.Producer,
        command.ReleaseDate);
}

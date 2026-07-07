using Conexa.Application.Movies.Common;
using MediatR;

namespace Conexa.Application.Movies.Commands.UpdateMovie;

public record UpdateMovieCommand(
    int Id,
    string Title,
    int? EpisodeId,
    string? OpeningCrawl,
    string? Director,
    string? Producer,
    DateOnly? ReleaseDate) : IRequest<MovieDto>, IMovieDetailsSource;

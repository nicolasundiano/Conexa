using Conexa.Application.Movies.Common;
using MediatR;

namespace Conexa.Application.Movies.Commands.CreateMovie;

public record CreateMovieCommand(
    string Title,
    int? EpisodeId,
    string? OpeningCrawl,
    string? Director,
    string? Producer,
    DateOnly? ReleaseDate) : IRequest<MovieDto>, IMovieDetailsCommand;

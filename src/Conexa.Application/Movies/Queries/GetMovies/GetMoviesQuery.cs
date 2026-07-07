using Conexa.Application.Movies.Common;
using MediatR;

namespace Conexa.Application.Movies.Queries.GetMovies;

public record GetMoviesQuery : IRequest<List<MovieListItemDto>>;

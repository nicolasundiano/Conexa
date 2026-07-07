using Conexa.Application.Movies.Common;
using MediatR;

namespace Conexa.Application.Movies.Queries.GetMovieById;

public record GetMovieByIdQuery(int Id) : IRequest<MovieDto>;

using Conexa.Application.Common.Pagination;
using Conexa.Application.Movies.Common;
using MediatR;

namespace Conexa.Application.Movies.Queries.GetMovies;

public record GetMoviesQuery(int Page = 1, int? PageSize = null) : IRequest<PagedList<MovieListItemDto>>;

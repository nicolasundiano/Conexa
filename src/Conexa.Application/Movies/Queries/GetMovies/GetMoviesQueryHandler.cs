using Conexa.Application.Common.Interfaces;
using Conexa.Application.Common.Pagination;
using Conexa.Application.Movies.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Movies.Queries.GetMovies;

public class GetMoviesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMoviesQuery, PagedList<MovieListItemDto>>
{
    public async Task<PagedList<MovieListItemDto>> Handle(GetMoviesQuery query, CancellationToken cancellationToken)
    {
        return await context.Movies
            .AsNoTracking()
            .OrderBy(m => m.EpisodeId == null)
            .ThenBy(m => m.EpisodeId)
            .ThenBy(m => m.Title)
            .Select(m => new MovieListItemDto(m.Id, m.Title, m.EpisodeId, m.Director, m.ReleaseDate))
            .ToPagedListAsync(query.Page, query.PageSize, cancellationToken);
    }
}

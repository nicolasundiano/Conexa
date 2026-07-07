using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Application.Movies.Common;
using Conexa.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Movies.Queries.GetMovieById;

public class GetMovieByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMovieByIdQuery, MovieDto>
{
    public async Task<MovieDto> Handle(GetMovieByIdQuery query, CancellationToken cancellationToken)
    {
        var movie = await context.Movies
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.Id == query.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Movie), query.Id);

        return MovieDto.From(movie);
    }
}

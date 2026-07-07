using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Application.Movies.Common;
using Conexa.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Movies.Commands.UpdateMovie;

public class UpdateMovieCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateMovieCommand, MovieDto>
{
    public async Task<MovieDto> Handle(UpdateMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await context.Movies
                        .FirstOrDefaultAsync(m => m.Id == command.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Movie), command.Id);

        movie.UpdateDetails(command.ToDetails());
        await context.SaveChangesAsync(cancellationToken);

        return MovieDto.From(movie);
    }
}

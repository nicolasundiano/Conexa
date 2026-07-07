using Conexa.Application.Common.Exceptions;
using Conexa.Application.Common.Interfaces;
using Conexa.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Movies.Commands.DeleteMovie;

public class DeleteMovieCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteMovieCommand>
{
    public async Task Handle(DeleteMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = await context.Movies
                        .FirstOrDefaultAsync(m => m.Id == command.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Movie), command.Id);

        context.Movies.Remove(movie);
        await context.SaveChangesAsync(cancellationToken);
    }
}

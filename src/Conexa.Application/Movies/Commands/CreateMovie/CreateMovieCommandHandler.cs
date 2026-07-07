using Conexa.Application.Common.Interfaces;
using Conexa.Application.Movies.Common;
using Conexa.Domain.Entities;
using MediatR;

namespace Conexa.Application.Movies.Commands.CreateMovie;

public class CreateMovieCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateMovieCommand, MovieDto>
{
    public async Task<MovieDto> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
    {
        var movie = Movie.Create(command.ToDetails());

        context.Movies.Add(movie);
        await context.SaveChangesAsync(cancellationToken);

        return MovieDto.From(movie);
    }
}

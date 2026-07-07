using Conexa.Application.Common.Interfaces;
using Conexa.Application.Movies.Common;
using Conexa.Domain.Common;
using Conexa.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Conexa.Application.Movies.Commands.SyncMovies;

public class SyncMoviesCommandHandler(
    IApplicationDbContext context,
    ISwapiClient swapiClient,
    ILogger<SyncMoviesCommandHandler> logger) : IRequestHandler<SyncMoviesCommand, SyncResultDto>
{
    public async Task<SyncResultDto> Handle(SyncMoviesCommand command, CancellationToken cancellationToken)
    {
        var films = await swapiClient.GetFilmsAsync(cancellationToken);
        var uniqueFilms = films.DistinctBy(f => f.Url).ToList();

        var syncedMovies = await context.Movies
            .Where(m => m.ExternalUrl != null)
            .ToDictionaryAsync(m => m.ExternalUrl!, cancellationToken);

        var created = 0;
        var updated = 0;
        var unchanged = 0;
        var skipped = films.Count - uniqueFilms.Count;

        foreach (var film in uniqueFilms)
        {
            if (string.IsNullOrWhiteSpace(film.Url))
            {
                skipped++;
                logger.LogWarning("Skipping film '{Title}': missing url", film.Title);
                continue;
            }

            try
            {
                var details = film.ToDetails();

                if (syncedMovies.TryGetValue(film.Url, out var movie))
                {
                    if (movie.SyncWith(details))
                        updated++;
                    else
                        unchanged++;
                }
                else
                {
                    context.Movies.Add(Movie.FromExternalSource(film.Url, details));
                    created++;
                }
            }
            catch (DomainException ex)
            {
                skipped++;
                logger.LogWarning("Skipping film '{Title}': {Reason}", film.Title, ex.Message);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return new SyncResultDto(created, updated, unchanged, skipped);
    }
}

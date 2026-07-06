using Conexa.Domain.Common;
using Conexa.Domain.ValueObjects;

namespace Conexa.Domain.Entities;

public class Movie : BaseAuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public int? EpisodeId { get; private set; }
    public string? OpeningCrawl { get; private set; }
    public string? Director { get; private set; }
    public string? Producer { get; private set; }
    public DateOnly? ReleaseDate { get; private set; }
    public string? ExternalUrl { get; private set; }

    public bool IsFromExternalSource => ExternalUrl is not null;

    private Movie() { }

    public static Movie Create(MovieDetails details)
    {
        var movie = new Movie();
        return movie.Apply(details);
    }

    public static Movie FromExternalSource(string externalUrl, MovieDetails details)
    {
        if (string.IsNullOrWhiteSpace(externalUrl))
            throw new DomainException("External url is required for imported movies.");

        var movie = new Movie { ExternalUrl = externalUrl };
        return movie.Apply(details);
    }

    public void UpdateDetails(MovieDetails details) => Apply(details);

    public bool SyncWith(MovieDetails details)
    {
        if (!IsFromExternalSource)
            throw new DomainException("Only movies imported from an external source can be synced.");

        if (CurrentDetails() == details) return false;

        Apply(details);
        return true;
    }

    private MovieDetails CurrentDetails() =>
        new(Title, EpisodeId, OpeningCrawl, Director, Producer, ReleaseDate);

    private Movie Apply(MovieDetails details)
    {
        Title = details.Title;
        EpisodeId = details.EpisodeId;
        OpeningCrawl = details.OpeningCrawl;
        Director = details.Director;
        Producer = details.Producer;
        ReleaseDate = details.ReleaseDate;
        return this;
    }
}

using Conexa.Domain.Common;

namespace Conexa.Domain.ValueObjects;

public sealed record MovieDetails
{
    public const int TitleMaxLength = 200;
    public const int DirectorMaxLength = 200;
    public const int ProducerMaxLength = 200;

    public string Title { get; }
    public int? EpisodeId { get; }
    public string? OpeningCrawl { get; }
    public string? Director { get; }
    public string? Producer { get; }
    public DateOnly? ReleaseDate { get; }

    public MovieDetails(
        string title,
        int? episodeId = null,
        string? openingCrawl = null,
        string? director = null,
        string? producer = null,
        DateOnly? releaseDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Movie title is required.");
        if (title.Trim().Length > TitleMaxLength)
            throw new DomainException($"Movie title cannot exceed {TitleMaxLength} characters.");
        if (episodeId is <= 0)
            throw new DomainException("Episode id must be a positive number.");
        if (director is { Length: > DirectorMaxLength })
            throw new DomainException($"Director cannot exceed {DirectorMaxLength} characters.");
        if (producer is { Length: > ProducerMaxLength })
            throw new DomainException($"Producer cannot exceed {ProducerMaxLength} characters.");

        Title = title.Trim();
        EpisodeId = episodeId;
        OpeningCrawl = openingCrawl;
        Director = director;
        Producer = producer;
        ReleaseDate = releaseDate;
    }
}

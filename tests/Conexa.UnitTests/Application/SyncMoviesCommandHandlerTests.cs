using Conexa.Application.Common.Interfaces;
using Conexa.Application.Movies.Commands.SyncMovies;
using Conexa.Domain.Entities;
using Conexa.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Xunit;

namespace Conexa.UnitTests.Application;

public class SyncMoviesCommandHandlerTests
{
    private readonly IApplicationDbContext _context = Substitute.For<IApplicationDbContext>();
    private readonly ISwapiClient _swapiClient = Substitute.For<ISwapiClient>();
    private readonly ILogger<SyncMoviesCommandHandler> _logger = Substitute.For<ILogger<SyncMoviesCommandHandler>>();

    private SyncMoviesCommandHandler CreateHandler(params Movie[] existingMovies)
    {
        var moviesDbSet = existingMovies.BuildMockDbSet();
        _context.Movies.Returns(moviesDbSet);
        return new SyncMoviesCommandHandler(_context, _swapiClient, _logger);
    }

    private static SwapiFilm Film(string title, string url, int? episodeId = 4) =>
        new(title, episodeId, null, "George Lucas", "Gary Kurtz", new DateOnly(1977, 5, 25), url);

    [Fact]
    public async Task Handle_WithNewFilms_CreatesThem()
    {
        _swapiClient.GetFilmsAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            Film("A New Hope", "https://swapi.tech/api/films/1"),
            Film("The Empire Strikes Back", "https://swapi.tech/api/films/2", 5)
        });
        var handler = CreateHandler();

        var result = await handler.Handle(new SyncMoviesCommand(), default);

        Assert.Equal(2, result.Created);
        Assert.Equal(0, result.Skipped);
        _context.Movies.Received(2).Add(Arg.Any<Movie>());
        await _context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithChangedExistingFilm_CountsAsUpdated()
    {
        var existing = Movie.FromExternalSource(
            "https://swapi.tech/api/films/1",
            new MovieDetails("Old Title", 4, director: "George Lucas"));
        _swapiClient.GetFilmsAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            Film("New Title", "https://swapi.tech/api/films/1")
        });
        var handler = CreateHandler(existing);

        var result = await handler.Handle(new SyncMoviesCommand(), default);

        Assert.Equal(1, result.Updated);
        Assert.Equal(0, result.Created);
        Assert.Equal("New Title", existing.Title);
    }

    [Fact]
    public async Task Handle_WithUnchangedExistingFilm_CountsAsUnchanged()
    {
        var details = new MovieDetails(
            "A New Hope", 4, null, "George Lucas", "Gary Kurtz", new DateOnly(1977, 5, 25));
        var existing = Movie.FromExternalSource("https://swapi.tech/api/films/1", details);
        _swapiClient.GetFilmsAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            Film("A New Hope", "https://swapi.tech/api/films/1")
        });
        var handler = CreateHandler(existing);

        var result = await handler.Handle(new SyncMoviesCommand(), default);

        Assert.Equal(1, result.Unchanged);
        Assert.Equal(0, result.Updated);
    }

    [Fact]
    public async Task Handle_WithDuplicateUrlsInPayload_SkipsDuplicates()
    {
        _swapiClient.GetFilmsAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            Film("A New Hope", "https://swapi.tech/api/films/1"),
            Film("A New Hope (dup)", "https://swapi.tech/api/films/1")
        });
        var handler = CreateHandler();

        var result = await handler.Handle(new SyncMoviesCommand(), default);

        Assert.Equal(1, result.Created);
        Assert.Equal(1, result.Skipped);
    }

    [Fact]
    public async Task Handle_WithMalformedFilm_SkipsItAndKeepsGoing()
    {
        _swapiClient.GetFilmsAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            Film("", "https://swapi.tech/api/films/1"),
            Film("The Empire Strikes Back", "https://swapi.tech/api/films/2", 5)
        });
        var handler = CreateHandler();

        var result = await handler.Handle(new SyncMoviesCommand(), default);

        Assert.Equal(1, result.Created);
        Assert.Equal(1, result.Skipped);
    }
}

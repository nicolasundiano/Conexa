using Conexa.Domain.Common;
using Conexa.Domain.Entities;
using Conexa.Domain.ValueObjects;
using Xunit;

namespace Conexa.UnitTests.Domain;

public class MovieTests
{
    private static MovieDetails Details(string title = "A New Hope", int? episodeId = 4) =>
        new(title, episodeId, director: "George Lucas");

    [Fact]
    public void Create_ProducesManualMovie()
    {
        var movie = Movie.Create(Details());

        Assert.False(movie.IsFromExternalSource);
        Assert.Null(movie.ExternalUrl);
        Assert.Equal("A New Hope", movie.Title);
    }

    [Fact]
    public void FromExternalSource_ProducesImportedMovie()
    {
        var movie = Movie.FromExternalSource("https://swapi.tech/api/films/1", Details());

        Assert.True(movie.IsFromExternalSource);
        Assert.Equal("https://swapi.tech/api/films/1", movie.ExternalUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FromExternalSource_WithEmptyUrl_Throws(string url)
    {
        Assert.Throws<DomainException>(() => Movie.FromExternalSource(url, Details()));
    }

    [Fact]
    public void SyncWith_WhenDetailsChanged_ReturnsTrueAndUpdates()
    {
        var movie = Movie.FromExternalSource("https://swapi.tech/api/films/1", Details(title: "Old Title"));

        var changed = movie.SyncWith(Details(title: "New Title"));

        Assert.True(changed);
        Assert.Equal("New Title", movie.Title);
    }

    [Fact]
    public void SyncWith_WhenDetailsUnchanged_ReturnsFalse()
    {
        var movie = Movie.FromExternalSource("https://swapi.tech/api/films/1", Details());

        var changed = movie.SyncWith(Details());

        Assert.False(changed);
    }

    [Fact]
    public void SyncWith_OnManualMovie_Throws()
    {
        var movie = Movie.Create(Details());

        Assert.Throws<DomainException>(() => movie.SyncWith(Details(title: "Other")));
    }

    [Fact]
    public void UpdateDetails_ChangesTitle()
    {
        var movie = Movie.Create(Details(title: "Old"));

        movie.UpdateDetails(Details(title: "New"));

        Assert.Equal("New", movie.Title);
    }
}

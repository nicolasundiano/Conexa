using Conexa.Domain.Common;
using Conexa.Domain.ValueObjects;
using Xunit;

namespace Conexa.UnitTests.Domain;

public class MovieDetailsTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesDetails()
    {
        var details = new MovieDetails("A New Hope", 4, director: "George Lucas");

        Assert.Equal("A New Hope", details.Title);
        Assert.Equal(4, details.EpisodeId);
        Assert.Equal("George Lucas", details.Director);
    }

    [Fact]
    public void Constructor_TrimsTitle()
    {
        var details = new MovieDetails("  A New Hope  ");

        Assert.Equal("A New Hope", details.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTitle_Throws(string? title)
    {
        Assert.Throws<DomainException>(() => new MovieDetails(title!));
    }

    [Fact]
    public void Constructor_WithTitleOverMaxLength_Throws()
    {
        var longTitle = new string('a', MovieDetails.TitleMaxLength + 1);

        Assert.Throws<DomainException>(() => new MovieDetails(longTitle));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveEpisodeId_Throws(int episodeId)
    {
        Assert.Throws<DomainException>(() => new MovieDetails("Title", episodeId));
    }

    [Fact]
    public void Constructor_WithDirectorOverMaxLength_Throws()
    {
        var longDirector = new string('a', MovieDetails.DirectorMaxLength + 1);

        Assert.Throws<DomainException>(() => new MovieDetails("Title", director: longDirector));
    }

    [Fact]
    public void Equality_IsByValue()
    {
        var a = new MovieDetails("Title", 4, "crawl", "director", "producer", new DateOnly(1977, 5, 25));
        var b = new MovieDetails("Title", 4, "crawl", "director", "producer", new DateOnly(1977, 5, 25));

        Assert.Equal(a, b);
    }
}

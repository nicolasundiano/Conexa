using Conexa.Application.Movies.Commands.CreateMovie;
using Conexa.Domain.ValueObjects;
using Xunit;

namespace Conexa.UnitTests.Application;

public class CreateMovieCommandValidatorTests
{
    private readonly CreateMovieCommandValidator _validator = new();

    private static CreateMovieCommand Command(string title = "A New Hope", int? episodeId = 4) =>
        new(title, episodeId, null, "George Lucas", "Gary Kurtz", new DateOnly(1977, 5, 25));

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var result = _validator.Validate(Command());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyTitle_Fails(string title)
    {
        var result = _validator.Validate(Command(title: title));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithNonPositiveEpisodeId_Fails()
    {
        var result = _validator.Validate(Command(episodeId: 0));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithTitleOverMaxLength_Fails()
    {
        var result = _validator.Validate(Command(title: new string('a', MovieDetails.TitleMaxLength + 1)));

        Assert.False(result.IsValid);
    }
}

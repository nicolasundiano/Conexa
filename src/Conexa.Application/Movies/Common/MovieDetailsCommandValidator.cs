using Conexa.Domain.ValueObjects;
using FluentValidation;

namespace Conexa.Application.Movies.Common;

public abstract class MovieDetailsCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : IMovieDetailsSource
{
    protected MovieDetailsCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(MovieDetails.TitleMaxLength);

        RuleFor(c => c.EpisodeId)
            .GreaterThan(0)
            .When(c => c.EpisodeId.HasValue);

        RuleFor(c => c.Director)
            .MaximumLength(MovieDetails.DirectorMaxLength);

        RuleFor(c => c.Producer)
            .MaximumLength(MovieDetails.ProducerMaxLength);
    }
}

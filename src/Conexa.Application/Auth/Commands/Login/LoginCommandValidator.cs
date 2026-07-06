using FluentValidation;

namespace Conexa.Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(c => c.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}

using Conexa.Application.Auth.Common;
using MediatR;

namespace Conexa.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : IRequest<AuthResponse>;

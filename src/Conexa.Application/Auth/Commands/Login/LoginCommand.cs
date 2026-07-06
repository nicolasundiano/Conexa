using Conexa.Application.Auth.Common;
using MediatR;

namespace Conexa.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

using Conexa.Api.Contracts;
using Conexa.Application.Auth.Commands.Login;
using Conexa.Application.Auth.Commands.RegisterUser;
using Conexa.Application.Auth.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conexa.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new RegisterUserCommand(request.Email, request.Password), cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return Ok(response);
    }
}

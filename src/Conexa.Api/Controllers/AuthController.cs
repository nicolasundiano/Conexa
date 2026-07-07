using Conexa.Api.Contracts;
using Conexa.Application.Auth.Commands.Login;
using Conexa.Application.Auth.Commands.RegisterUser;
using Conexa.Application.Auth.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conexa.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Registra un nuevo usuario (rol Regular) y devuelve un token de acceso.</summary>
    /// <response code="200">Usuario creado; devuelve el token de acceso.</response>
    /// <response code="400">Datos inválidos (email mal formado o contraseña débil).</response>
    /// <response code="409">Ya existe un usuario con ese email.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new RegisterUserCommand(request.Email, request.Password), cancellationToken);
        return Ok(response);
    }

    /// <summary>Autentica un usuario y devuelve un token de acceso.</summary>
    /// <response code="200">Credenciales válidas; devuelve el token de acceso.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">Email o contraseña incorrectos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
        return Ok(response);
    }
}

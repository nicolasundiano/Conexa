using Conexa.Api.Contracts;
using Conexa.Application.Common.Pagination;
using Conexa.Application.Movies.Commands.CreateMovie;
using Conexa.Application.Movies.Commands.DeleteMovie;
using Conexa.Application.Movies.Commands.SyncMovies;
using Conexa.Application.Movies.Commands.UpdateMovie;
using Conexa.Application.Movies.Common;
using Conexa.Application.Movies.Queries.GetMovieById;
using Conexa.Application.Movies.Queries.GetMovies;
using Conexa.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Conexa.Api.Controllers;

[ApiController]
[Route("api/movies")]
[Authorize]
[Produces("application/json")]
public class MoviesController(ISender sender) : ControllerBase
{
    /// <summary>Lista las películas de forma paginada. Disponible para cualquier usuario autenticado.</summary>
    /// <param name="page">Número de página (por defecto 1).</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10, máximo 100).</param>
    /// <response code="200">Página de películas con metadata de paginación.</response>
    /// <response code="401">No autenticado.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<MovieListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedList<MovieListItemDto>>> GetMovies(
        [FromQuery] int page = 1,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        return Ok(await sender.Send(new GetMoviesQuery(page, pageSize), cancellationToken));
    }

    /// <summary>Obtiene el detalle de una película. Exclusivo de usuarios Regular.</summary>
    /// <response code="200">Detalle de la película.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">El usuario no tiene el rol Regular.</response>
    /// <response code="404">La película no existe.</response>
    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Regular))]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDto>> GetMovieById(int id, CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetMovieByIdQuery(id), cancellationToken));
    }

    /// <summary>Crea una nueva película. Exclusivo de administradores.</summary>
    /// <response code="201">Película creada.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">El usuario no tiene el rol Admin.</response>
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MovieDto>> Create(CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await sender.Send(
            new CreateMovieCommand(
                request.Title, request.EpisodeId, request.OpeningCrawl,
                request.Director, request.Producer, request.ReleaseDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
    }

    /// <summary>Actualiza una película existente. Exclusivo de administradores.</summary>
    /// <response code="200">Película actualizada.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">El usuario no tiene el rol Admin.</response>
    /// <response code="404">La película no existe.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDto>> Update(int id, UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await sender.Send(
            new UpdateMovieCommand(
                id, request.Title, request.EpisodeId, request.OpeningCrawl,
                request.Director, request.Producer, request.ReleaseDate),
            cancellationToken);

        return Ok(movie);
    }

    /// <summary>Elimina una película. Exclusivo de administradores.</summary>
    /// <response code="204">Película eliminada.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">El usuario no tiene el rol Admin.</response>
    /// <response code="404">La película no existe.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteMovieCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Sincroniza el catálogo local con las películas de la Star Wars API. Exclusivo de administradores.</summary>
    /// <response code="200">Resultado de la sincronización (creadas, actualizadas, sin cambios, omitidas).</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">El usuario no tiene el rol Admin.</response>
    /// <response code="502">La Star Wars API no está disponible.</response>
    [HttpPost("sync")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<SyncResultDto>> Sync(CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new SyncMoviesCommand(), cancellationToken));
    }
}

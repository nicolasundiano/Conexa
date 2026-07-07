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
public class MoviesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedList<MovieListItemDto>>> GetMovies(
        [FromQuery] int page = 1,
        [FromQuery] int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        return Ok(await sender.Send(new GetMoviesQuery(page, pageSize), cancellationToken));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Regular))]
    public async Task<ActionResult<MovieDto>> GetMovieById(int id, CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetMovieByIdQuery(id), cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<MovieDto>> Create(CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await sender.Send(
            new CreateMovieCommand(
                request.Title, request.EpisodeId, request.OpeningCrawl,
                request.Director, request.Producer, request.ReleaseDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<MovieDto>> Update(int id, UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await sender.Send(
            new UpdateMovieCommand(
                id, request.Title, request.EpisodeId, request.OpeningCrawl,
                request.Director, request.Producer, request.ReleaseDate),
            cancellationToken);

        return Ok(movie);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteMovieCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("sync")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<SyncResultDto>> Sync(CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new SyncMoviesCommand(), cancellationToken));
    }
}

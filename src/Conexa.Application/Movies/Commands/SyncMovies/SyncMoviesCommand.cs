using Conexa.Application.Movies.Common;
using MediatR;

namespace Conexa.Application.Movies.Commands.SyncMovies;

public record SyncMoviesCommand : IRequest<SyncResultDto>;

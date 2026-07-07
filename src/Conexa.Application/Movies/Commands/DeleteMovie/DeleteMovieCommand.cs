using MediatR;

namespace Conexa.Application.Movies.Commands.DeleteMovie;

public record DeleteMovieCommand(int Id) : IRequest;

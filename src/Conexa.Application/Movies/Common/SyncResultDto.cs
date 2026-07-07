namespace Conexa.Application.Movies.Common;

public record SyncResultDto(int Created, int Updated, int Unchanged, int Skipped);

using Microsoft.EntityFrameworkCore;

namespace Conexa.Application.Common.Pagination;

public static class QueryableExtensions
{
    private const int MinPage = 1;
    private const int DefaultPage = 1;
    private const int MinPageSize = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    public static async Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var validPage = page < MinPage ? DefaultPage : page;
        var size = Math.Clamp(pageSize ?? DefaultPageSize, MinPageSize, MaxPageSize);

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(validPage - 1) * size;

        var items = skip >= totalCount
            ? []
            : await query.Skip((int)skip).Take(size).ToListAsync(cancellationToken);

        return PagedList<T>.Create(items, validPage, size, totalCount);
    }
}

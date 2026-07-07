using Conexa.Application.Common.Pagination;
using MockQueryable;
using Xunit;

namespace Conexa.UnitTests.Application;

public class PagedListTests
{
    private sealed record Item(int Value);

    private static IQueryable<Item> Source(int count) =>
        Enumerable.Range(1, count).Select(i => new Item(i)).ToList().BuildMock();

    [Fact]
    public async Task ToPagedListAsync_ReturnsRequestedPage()
    {
        var page = await Source(25).ToPagedListAsync(page: 2, pageSize: 10);

        Assert.Equal(2, page.Page);
        Assert.Equal(10, page.PageSize);
        Assert.Equal(25, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.True(page.HasPrevious);
        Assert.True(page.HasNext);
        Assert.Equal(Enumerable.Range(11, 10), page.Items.Select(i => i.Value));
    }

    [Fact]
    public async Task ToPagedListAsync_ClampsPageSizeToMax()
    {
        var page = await Source(500).ToPagedListAsync(page: 1, pageSize: 99999);

        Assert.Equal(100, page.PageSize);
        Assert.Equal(100, page.Items.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task ToPagedListAsync_WithInvalidPage_DefaultsToFirst(int page)
    {
        var result = await Source(10).ToPagedListAsync(page, pageSize: 5);

        Assert.Equal(1, result.Page);
        Assert.Equal(Enumerable.Range(1, 5), result.Items.Select(i => i.Value));
    }

    [Fact]
    public async Task ToPagedListAsync_WithPageBeyondRange_ReturnsEmpty()
    {
        var page = await Source(6).ToPagedListAsync(page: 999, pageSize: 10);

        Assert.Empty(page.Items);
        Assert.Equal(6, page.TotalCount);
    }

    [Fact]
    public async Task ToPagedListAsync_WithHugePage_DoesNotOverflow()
    {
        var page = await Source(6).ToPagedListAsync(page: int.MaxValue, pageSize: 100);

        Assert.Empty(page.Items);
    }
}

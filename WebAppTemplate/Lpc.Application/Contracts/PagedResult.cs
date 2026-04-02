namespace Lpc.Application.Contracts;

public sealed class PagedResult<T>
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 500;

    public required int Page { get; init; }          // 1-based
    public required int PageSize { get; init; }
    public required int TotalItems { get; init; }
    public required IReadOnlyList<T> Items { get; init; }

    
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public static PagedResult<T> Empty => new()
    {
        Page = DefaultPage,
        PageSize = DefaultPageSize,
        TotalItems = 0,
        Items = []
    };
}

public static class PagedResultExtensions
{
    public static PagedResult<T> ToPagedResult<T>(
        this IEnumerable<T> source,
        int page,
        int pageSize)
    {
        if (page <= 0) page = PagedResult<T>.DefaultPage;
        if (pageSize <= 0) pageSize = PagedResult<T>.DefaultPageSize;

        pageSize = Math.Min(pageSize, PagedResult<T>.MaxPageSize);

        var list = source as IList<T> ?? [.. source];

        var totalItems = list.Count;

        var items = list
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            Items = items
        };
    }
}

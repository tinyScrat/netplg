namespace Lpc.Contracts;

public sealed class PagedResult<T>
{
    public required int Page { get; init; }          // 1-based
    public required int PageSize { get; init; }
    public required int TotalItems { get; init; }
    public required IReadOnlyList<T> Items { get; init; }

    
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public static PagedResult<T> Empty => new()
    {
        Page = 1,
        PageSize = 100,
        TotalItems = 0,
        Items = []
    };
}

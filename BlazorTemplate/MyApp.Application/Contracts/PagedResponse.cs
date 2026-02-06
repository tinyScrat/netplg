namespace MyApp.Contracts;

public sealed class PagedResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }

    public required int Page { get; init; }          // 1-based
    public required int PageSize { get; init; }

    public required int TotalItems { get; init; }
    public required int TotalPages { get; init; }

    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}

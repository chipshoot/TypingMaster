namespace TypingMaster.Core.Models;

public class PagedResult<T>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public IEnumerable<T> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public bool HasNextPage => Page < TotalPages;

    public bool HasPreviousPage => Page > 1;
}
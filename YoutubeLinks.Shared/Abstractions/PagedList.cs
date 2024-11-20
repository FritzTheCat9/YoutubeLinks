namespace YoutubeLinks.Shared.Abstractions;

public class PagedList<T>(
    List<T> items,
    int page,
    int pageSize,
    int totalCount)
{
    public List<T> Items { get; set; } = items;
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public int PagesCount => (int)double.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page * PageSize < TotalCount;
}
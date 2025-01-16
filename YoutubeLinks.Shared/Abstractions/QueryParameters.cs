namespace YoutubeLinks.Shared.Abstractions;

public class QueryParameters : IPagedQuery, ISortedQuery, IFilteredQuery
{
    public string SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortColumn { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.None;
}

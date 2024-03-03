namespace YoutubeLinks.Shared.Abstractions
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PagesCount => (int)double.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageSize > 1;
        public bool HasNextPage => Page * PageSize < TotalCount;

        public PagedList(
            List<T> items,
            int page,
            int pageSize,
            int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}

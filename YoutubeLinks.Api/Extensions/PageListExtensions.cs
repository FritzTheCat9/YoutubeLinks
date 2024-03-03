using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Extensions
{
    public static class PageListExtensions<T>
    {
        private static readonly int _minPage = 0;
        private static readonly int _minPageSize = 0;
        private static readonly int _maxPageSize = 100;

        public static PagedList<T> Create(
            IQueryable<T> source,
            int Page,
            int PageSize)
        {
            Validate(Page, PageSize);

            var totalCount = source.Count();
            var items = source.Skip((Page - 1) * PageSize)
                              .Take(PageSize)
                              .ToList();

            return new PagedList<T>(items, Page, PageSize, totalCount);
        }

        public static PagedList<T> CreateEmpty(
            int Page,
            int PageSize)
        {
            Validate(Page, PageSize);

            return new PagedList<T>([], Page, PageSize, 0);
        }

        private static void Validate(int Page, int PageSize)
        {
            if (Page <= _minPage)
                throw new MyValidationException(nameof(Page), $"{nameof(Page)} should be greater than {_minPage}");
            if (PageSize <= _minPageSize)
                throw new MyValidationException(nameof(PageSize), $"{nameof(PageSize)} should be greater than {_minPageSize}");
            if (PageSize > _maxPageSize)
                throw new MyValidationException(nameof(PageSize), $"{nameof(PageSize)} should be less than or equal to {_maxPageSize}");
        }
    }
}

using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Api.Extensions;

public static class PageListExtensions<T>
{
    private const int MinPage = 0;
    private const int MinPageSize = 0;
    private const int MaxPageSize = 100;

    public static PagedList<T> Create(
        IQueryable<T> source,
        int page,
        int pageSize)
    {
        Validate(page, pageSize);

        var totalCount = source.Count();
        var items = source.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>(items, page, pageSize, totalCount);
    }

    // TODO: refactor this code 
    public static PagedList<TResult> Convert<TSource, TResult>(
        PagedList<TSource> source,
        Func<TSource, TResult> mapFunc)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (mapFunc == null)
        {
            throw new ArgumentNullException(nameof(mapFunc));
        }

        var mappedItems = source.Items.Select(mapFunc).ToList();

        return new PagedList<TResult>(
            mappedItems,
            source.Page,
            source.PageSize,
            source.TotalCount);
    }

    public static PagedList<T> CreateEmpty(
        int page,
        int pageSize)
    {
        Validate(page, pageSize);

        return new PagedList<T>([], page, pageSize, 0);
    }

    // TODO: Move this validations to Query model
    private static void Validate(int page, int pageSize)
    {
        if (page <= MinPage)
        {
            throw new MyValidationException(nameof(page), $"{nameof(page)} should be greater than {MinPage}");
        }

        switch (pageSize)
        {
            case <= MinPageSize:
                throw new MyValidationException(nameof(pageSize),
                    $"{nameof(pageSize)} should be greater than {MinPageSize}");
            case > MaxPageSize:
                throw new MyValidationException(nameof(pageSize),
                    $"{nameof(pageSize)} should be less than or equal to {MaxPageSize}");
        }
    }
}
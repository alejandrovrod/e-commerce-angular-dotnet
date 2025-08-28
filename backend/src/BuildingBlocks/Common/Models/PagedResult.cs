namespace ECommerce.BuildingBlocks.Common.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public PagedResult()
    {
    }

    public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    }

    public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        return new PagedResult<T>(items, totalCount, page, pageSize);
    }

    public static PagedResult<T> Empty(int page = 1, int pageSize = 20)
    {
        return new PagedResult<T>
        {
            Items = new List<T>(),
            TotalCount = 0,
            Page = page,
            PageSize = pageSize,
            TotalPages = 0
        };
    }
}


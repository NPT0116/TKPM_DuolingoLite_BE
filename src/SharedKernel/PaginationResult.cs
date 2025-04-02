using System;

namespace SharedKernel;


public class PaginationResult<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginationResult(
        IReadOnlyCollection<T> items, 
        int pageNumber, 
        int pageSize, 
        int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static PaginationResult<T> Create(
        IReadOnlyCollection<T> items, 
        int pageNumber, 
        int pageSize, 
        int totalCount)
    {
        return new PaginationResult<T>(items, pageNumber, pageSize, totalCount);
    }
}
using System;

namespace Domain.Query;

public abstract class PaginationQueryParams
{

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchKeyword { get; set;} = string.Empty;}



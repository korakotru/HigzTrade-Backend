public class ProductQueryDto
{
    public sealed record Request(string? Keyword, int PageIndex = 1, int PageSize = 10);

    public sealed record Response(
        int ProductId,
        string Name,
        string Sku,
        decimal Price,
        string Status,
        int CategoryId,
        string CategoryName,
        string CreatedBy,     
        DateTime CreatedAt,   
        string UpdatedBy,     
        DateTime? UpdatedAt,  
        int StockQty          
    );                        

    // Wrapper สำหรับส่งกลับไปให้ React
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
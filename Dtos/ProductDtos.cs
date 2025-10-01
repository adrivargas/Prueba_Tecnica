namespace ProductService.Api.Dtos
{
    public record ProductCreateDto(
    string Name,
    string? Description,
    string? Category,
    string? ImageUrl,
    decimal Price,
    int Stock
    );


    public record ProductUpdateDto(
    string Name,
    string? Description,
    string? Category,
    string? ImageUrl,
    decimal Price,
    int Stock,
    byte[] RowVersion
    );


    public class ProductQuery
    {
        public string? Q { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = "id"; // id|name|price
        public string? SortDir { get; set; } = "asc"; // asc|desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
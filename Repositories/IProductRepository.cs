using ProductService.Api.Dtos;
using ProductService.Api.Entities;


namespace ProductService.Api.Repositories
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> data, int total)> GetAsync(ProductQuery q, CancellationToken ct);
        Task<Product?> GetByIdAsync(int id, CancellationToken ct);
        Task<Product> CreateAsync(ProductCreateDto dto, CancellationToken ct);
        Task<Product?> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
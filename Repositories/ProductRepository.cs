using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Dtos;
using ProductService.Api.Entities;


namespace ProductService.Api.Repositories
{
    public class ProductRepository(AppDbContext db) : IProductRepository
    {
        public async Task<(IEnumerable<Product> data, int total)> GetAsync(ProductQuery q, CancellationToken ct)
        {
            var query = db.Products.AsNoTracking();


            if (!string.IsNullOrWhiteSpace(q.Q))
                query = query.Where(p => p.Name.Contains(q.Q) || (p.Description ?? "").Contains(q.Q));


            if (!string.IsNullOrWhiteSpace(q.Category))
                query = query.Where(p => p.Category == q.Category);


            if (q.MinPrice.HasValue)
                query = query.Where(p => p.Price >= q.MinPrice);

            if (q.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= q.MaxPrice);


            query = (q.SortBy?.ToLower(), q.SortDir?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("price", _) => query.OrderBy(p => p.Price),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("name", _) => query.OrderBy(p => p.Name),
                _ => query.OrderBy(p => p.Id)
            };


            var total = await query.CountAsync(ct);
            var data = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct);
            return (data, total);
        }


        public Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);


        public async Task<Product> CreateAsync(ProductCreateDto dto, CancellationToken ct)
        {
            var entity = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                Price = dto.Price,
                Stock = dto.Stock
            };
            db.Products.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity;
        }


        public async Task<Product?> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct)
        {
            var entity = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (entity is null) return null;


            entity.Name = dto.Name.Trim();
            entity.Description = dto.Description;
            entity.Category = dto.Category;
            entity.ImageUrl = dto.ImageUrl;
            entity.Price = dto.Price;
            entity.Stock = dto.Stock;
            db.Entry(entity).Property(e => e.RowVersion).OriginalValue = dto.RowVersion;


            try
            {
                await db.SaveChangesAsync(ct);
                return entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw; // Será capturado en el controller para retornar 409
            }
        }


        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (entity is null) return false;
            db.Products.Remove(entity);
            await db.SaveChangesAsync(ct);
            return true;
        }
    }
}
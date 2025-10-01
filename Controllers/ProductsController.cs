using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Dtos;
using ProductService.Api.Entities;
using ProductService.Api.Repositories;


namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository repo) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery q, CancellationToken ct)
        {
            if (q.Page <= 0) q.Page = 1;
            if (q.PageSize <= 0 || q.PageSize > 100) q.PageSize = 10;
            var (data, total) = await repo.GetAsync(q, ct);
            return Ok(new { data, total, page = q.Page, pageSize = q.PageSize });
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id, CancellationToken ct)
        {
            var item = await repo.GetByIdAsync(id, ct);
            return item is null ? NotFound() : Ok(item);
        }


        [HttpGet("{id:int}/stock")]
        public async Task<IActionResult> GetStock(int id, CancellationToken ct)
        {
            var item = await repo.GetByIdAsync(id, ct);
            return item is null ? NotFound() : Ok(new { stock = item.Stock });
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name requerido");
            if (dto.Price < 0) return BadRequest("Price >= 0");
            if (dto.Stock < 0) return BadRequest("Stock >= 0");


            var created = await repo.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await repo.UpdateAsync(id, dto, ct);
                return updated is null ? NotFound() : Ok(updated);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { error = "Conflicto de concurrencia. Refresca el recurso." });
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await repo.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
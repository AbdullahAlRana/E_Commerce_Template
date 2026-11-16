using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _svc;
    public ProductsController(IProductService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null, [FromQuery] Guid? brandId = null, [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? featured = null, [FromQuery] string? sort = null, CancellationToken ct = default)
        => Ok(await _svc.GetAllAsync(page, pageSize, search, categoryId, brandId, minPrice, maxPrice, featured, sort, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct = default)
    {
        var r = await _svc.GetByIdAsync(id, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken ct = default)
        => Ok(await _svc.CreateAsync(request, ct));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var r = await _svc.UpdateAsync(id, request, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();
}
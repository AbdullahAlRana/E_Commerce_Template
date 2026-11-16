using ECommerce.Application.DTOs.Brands;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _svc;
    public BrandsController(IBrandService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default)
        => Ok(await _svc.GetAllAsync(page, pageSize, search, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct = default)
    {
        var r = await _svc.GetByIdAsync(id, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBrandRequest request, CancellationToken ct = default)
        => Ok(await _svc.CreateAsync(request, ct));

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBrandRequest request, CancellationToken ct = default)
    {
        var r = await _svc.UpdateAsync(id, request, ct);
        return r == null ? NotFound() : Ok(r);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
        => (await _svc.DeleteAsync(id, ct)) ? NoContent() : NotFound();
}
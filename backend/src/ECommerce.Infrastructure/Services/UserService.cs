using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Users;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _users;

    public UserService(UserManager<ApplicationUser> users) => _users = users;

    public async Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var q = _users.Users;
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(u => u.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var dtos = new List<UserDto>();
        foreach (var u in items)
        {
            var roles = await _users.GetRolesAsync(u);
            dtos.Add(new UserDto { Id = u.Id, Email = u.Email ?? string.Empty, FirstName = u.FirstName, LastName = u.LastName, Phone = u.PhoneNumber, Roles = roles, IsActive = u.IsActive, CreatedAt = u.CreatedAt });
        }
        return new PagedResult<UserDto> { Items = dtos, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<UserDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var u = await _users.FindByIdAsync(id);
        if (u == null) return null;
        var roles = await _users.GetRolesAsync(u);
        return new UserDto { Id = u.Id, Email = u.Email ?? string.Empty, FirstName = u.FirstName, LastName = u.LastName, Phone = u.PhoneNumber, Roles = roles, IsActive = u.IsActive, CreatedAt = u.CreatedAt };
    }

    public async Task<bool> SetActiveAsync(string id, bool active, CancellationToken ct = default)
    {
        var u = await _users.FindByIdAsync(id);
        if (u == null) return false;
        u.IsActive = active;
        await _users.UpdateAsync(u);
        return true;
    }

    public async Task<bool> AssignRoleAsync(string id, string role, CancellationToken ct = default)
    {
        var u = await _users.FindByIdAsync(id);
        if (u == null) return false;
        if (!await _users.IsInRoleAsync(u, role)) await _users.AddToRoleAsync(u, role);
        return true;
    }
}

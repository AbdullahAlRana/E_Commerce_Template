using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.DTOs.Addresses;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _repo;
    private readonly IUnitOfWork _uow;

    public AddressService(IAddressRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    private static AddressDto Map(Address a) => new() { Id = a.Id, FullName = a.FullName, Phone = a.Phone, Street = a.Street, City = a.City, State = a.State, ZipCode = a.ZipCode, Country = a.Country, IsDefault = a.IsDefault };

    public async Task<IReadOnlyList<AddressDto>> GetByUserAsync(string userId, CancellationToken ct = default)
    {
        var list = await _repo.GetByUserAsync(userId, ct);
        return list.Select(Map).ToList();
    }

    public async Task<AddressDto> CreateAsync(string userId, CreateAddressRequest request, CancellationToken ct = default)
    {
        if (request.IsDefault)
        {
            var existing = await _repo.GetByUserAsync(userId, ct);
            foreach (var e in existing) { e.IsDefault = false; _repo.Update(e); }
        }
        var a = new Address
        {
            UserId = userId,
            FullName = request.FullName,
            Phone = request.Phone,
            Street = request.Street,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country,
            IsDefault = request.IsDefault
        };
        await _repo.AddAsync(a, ct);
        await _uow.SaveChangesAsync(ct);
        return Map(a);
    }

    public async Task<AddressDto?> UpdateAsync(string userId, Guid id, CreateAddressRequest request, CancellationToken ct = default)
    {
        var a = await _repo.GetByIdAsync(id, ct);
        if (a == null || a.UserId != userId) return null;
        if (request.IsDefault)
        {
            var existing = await _repo.GetByUserAsync(userId, ct);
            foreach (var e in existing.Where(x => x.Id != id)) { e.IsDefault = false; _repo.Update(e); }
        }
        a.FullName = request.FullName; a.Phone = request.Phone; a.Street = request.Street; a.City = request.City; a.State = request.State; a.ZipCode = request.ZipCode; a.Country = request.Country; a.IsDefault = request.IsDefault; a.UpdatedAt = DateTime.UtcNow;
        _repo.Update(a);
        await _uow.SaveChangesAsync(ct);
        return Map(a);
    }

    public async Task<bool> DeleteAsync(string userId, Guid id, CancellationToken ct = default)
    {
        var a = await _repo.GetByIdAsync(id, ct);
        if (a == null || a.UserId != userId) return false;
        _repo.Remove(a);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}

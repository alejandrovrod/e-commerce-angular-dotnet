using Microsoft.EntityFrameworkCore;
using ECommerce.Payment.Application.Interfaces;
using ECommerce.Payment.Domain.Entities;
using ECommerce.Payment.Domain.Enums;
using ECommerce.Payment.Infrastructure.Data;

namespace ECommerce.Payment.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<ECommerce.Payment.Domain.Entities.Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ECommerce.Payment.Domain.Entities.Payment?> GetByPaymentNumberAsync(string paymentNumber)
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber);
    }

    public async Task<ECommerce.Payment.Domain.Entities.Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetByStatusAsync(PaymentStatus status)
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetByGatewayAsync(PaymentGateway gateway)
    {
        return await _context.Payments
            .Include(p => p.Attempts)
            .Include(p => p.Refunds)
            .Where(p => p.Gateway == gateway)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<ECommerce.Payment.Domain.Entities.Payment> AddAsync(ECommerce.Payment.Domain.Entities.Payment payment)
    {
        var entry = await _context.Payments.AddAsync(payment);
        return entry.Entity;
    }

    public async Task<ECommerce.Payment.Domain.Entities.Payment> UpdateAsync(ECommerce.Payment.Domain.Entities.Payment payment)
    {
        var entry = _context.Payments.Update(payment);
        return entry.Entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var payment = await GetByIdAsync(id);
        if (payment != null)
        {
            _context.Payments.Remove(payment);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Payments.AnyAsync(p => p.Id == id);
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Payments.CountAsync();
    }
}

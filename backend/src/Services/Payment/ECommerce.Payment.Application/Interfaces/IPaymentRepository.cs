using ECommerce.Payment.Domain.Entities;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Interfaces;

public interface IPaymentRepository
{
    Task<ECommerce.Payment.Domain.Entities.Payment?> GetByIdAsync(Guid id);
    Task<ECommerce.Payment.Domain.Entities.Payment?> GetByPaymentNumberAsync(string paymentNumber);
    Task<ECommerce.Payment.Domain.Entities.Payment?> GetByOrderIdAsync(Guid orderId);
    Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetByUserIdAsync(Guid userId);
    Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetAllAsync();
    Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetByStatusAsync(PaymentStatus status);
    Task<List<ECommerce.Payment.Domain.Entities.Payment>> GetByGatewayAsync(PaymentGateway gateway);
    Task<ECommerce.Payment.Domain.Entities.Payment> AddAsync(ECommerce.Payment.Domain.Entities.Payment payment);
    Task<ECommerce.Payment.Domain.Entities.Payment> UpdateAsync(ECommerce.Payment.Domain.Entities.Payment payment);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetCountAsync();
}

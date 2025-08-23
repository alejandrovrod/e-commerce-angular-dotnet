using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Domain.Entities;

public class Review : BaseAuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = default!;
    public string? UserAvatar { get; private set; }
    public decimal Rating { get; private set; }
    public string Title { get; private set; } = default!;
    public string Comment { get; private set; } = default!;
    public List<string>? Pros { get; private set; }
    public List<string>? Cons { get; private set; }
    public bool IsVerifiedPurchase { get; private set; }
    public int HelpfulCount { get; private set; } = 0;
    public List<string>? Images { get; private set; }
    public ReviewStatus Status { get; private set; } = ReviewStatus.Pending;

    private Review() { } // For EF Core

    public static Review Create(
        Guid productId,
        Guid userId,
        string userName,
        decimal rating,
        string title,
        string comment,
        bool isVerifiedPurchase = false)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        return new Review
        {
            ProductId = productId,
            UserId = userId,
            UserName = userName,
            Rating = rating,
            Title = title,
            Comment = comment,
            IsVerifiedPurchase = isVerifiedPurchase,
            Status = ReviewStatus.Pending
        };
    }

    public void Approve()
    {
        Status = ReviewStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = ReviewStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddHelpful()
    {
        HelpfulCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}




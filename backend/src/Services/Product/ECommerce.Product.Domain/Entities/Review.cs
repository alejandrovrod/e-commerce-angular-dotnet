using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Domain.Entities;

public class Review : BaseAuditableEntity
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public ReviewStatus Status { get; private set; }
    
    // Navigation properties
    public Product Product { get; private set; } = null!;
    
    // Private constructor for EF Core
    private Review() { }
    
    public Review(Guid productId, Guid userId, string title, string content, int rating)
    {
        ProductId = productId;
        UserId = userId;
        Title = title;
        Content = content;
        Rating = rating;
        Status = ReviewStatus.Pending;
    }
    
    public void Update(string title, string content, int rating)
    {
        Title = title;
        Content = content;
        Rating = rating;
        Status = ReviewStatus.Pending; // Reset to pending when updated
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Approve()
    {
        Status = ReviewStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Reject(string reason)
    {
        Status = ReviewStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");
        
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsApproved => Status == ReviewStatus.Approved;
    public bool IsPending => Status == ReviewStatus.Pending;
    public bool IsRejected => Status == ReviewStatus.Rejected;
}






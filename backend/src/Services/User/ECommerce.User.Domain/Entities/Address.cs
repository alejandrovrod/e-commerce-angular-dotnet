using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Domain.Enums;

namespace ECommerce.User.Domain.Entities;

public class Address : BaseEntity
{
    public Guid UserId { get; private set; }
    public AddressType Type { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? Company { get; private set; }
    public string Address1 { get; private set; } = default!;
    public string? Address2 { get; private set; }
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string ZipCode { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string? Phone { get; private set; }
    public bool IsDefault { get; private set; } = false;

    // Navigation properties
    public virtual User User { get; private set; } = default!;

    private Address() { } // For EF Core

    public static Address Create(
        Guid userId,
        AddressType type,
        string firstName,
        string lastName,
        string address1,
        string city,
        string state,
        string zipCode,
        string country,
        string? company = null,
        string? address2 = null,
        string? phone = null,
        bool isDefault = false)
    {
        return new Address
        {
            UserId = userId,
            Type = type,
            FirstName = firstName,
            LastName = lastName,
            Company = company,
            Address1 = address1,
            Address2 = address2,
            City = city,
            State = state,
            ZipCode = zipCode,
            Country = country,
            Phone = phone,
            IsDefault = isDefault
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string address1,
        string city,
        string state,
        string zipCode,
        string country,
        string? company = null,
        string? address2 = null,
        string? phone = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Company = company;
        Address1 = address1;
        Address2 = address2;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAsDefault()
    {
        IsDefault = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFullAddress()
    {
        var parts = new List<string> { Address1 };
        
        if (!string.IsNullOrEmpty(Address2))
            parts.Add(Address2);
            
        parts.Add($"{City}, {State} {ZipCode}");
        parts.Add(Country);
        
        return string.Join(", ", parts);
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}











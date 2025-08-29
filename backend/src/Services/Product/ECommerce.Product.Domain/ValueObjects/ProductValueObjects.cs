namespace ECommerce.Product.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = "USD")
{
    public static Money Zero => new(0);
    public static Money Create(decimal amount, string currency = "USD") => new(amount, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        return new Money(Amount - other.Amount, Currency);
    }
    
    public Money Multiply(decimal factor) => new(Amount * factor, Currency);
    
    public bool IsPositive() => Amount > 0;
    public bool IsZero() => Amount == 0;
    public bool IsNegative() => Amount < 0;
}

public record Weight(decimal Value, string Unit = "kg")
{
    public static Weight FromKg(decimal kg) => new(kg, "kg");
    public static Weight FromLb(decimal lb) => new(lb, "lb");
    public static Weight FromG(decimal g) => new(g, "g");
    
    public Weight ConvertTo(string unit)
    {
        return unit.ToLower() switch
        {
            "kg" when Unit == "lb" => new Weight(Value * 0.453592m, "kg"),
            "lb" when Unit == "kg" => new Weight(Value * 2.20462m, "lb"),
            "g" when Unit == "kg" => new Weight(Value * 1000, "g"),
            "kg" when Unit == "g" => new Weight(Value / 1000, "kg"),
            _ => this
        };
    }
}

public record Dimensions(decimal Length, decimal Width, decimal Height, string Unit = "cm")
{
    public decimal Volume => Length * Width * Height;
    
    public Dimensions ConvertTo(string unit)
    {
        return unit.ToLower() switch
        {
            "in" when Unit == "cm" => new Dimensions(Length * 0.393701m, Width * 0.393701m, Height * 0.393701m, "in"),
            "cm" when Unit == "in" => new Dimensions(Length * 2.54m, Width * 2.54m, Height * 2.54m, "cm"),
            _ => this
        };
    }
}

public record Inventory(int Stock, int LowStockThreshold = 0, bool TrackQuantity = true, bool AllowBackorder = false)
{
    public bool IsInStock() => !TrackQuantity || Stock > 0 || AllowBackorder;
    public bool IsLowStock() => TrackQuantity && Stock <= LowStockThreshold && Stock > 0;
    public bool IsOutOfStock() => TrackQuantity && Stock <= 0 && !AllowBackorder;
    
    public Inventory ReduceStock(int quantity)
    {
        if (!TrackQuantity) return this;
        return this with { Stock = Math.Max(0, Stock - quantity) };
    }
    
    public Inventory AddStock(int quantity)
    {
        if (!TrackQuantity) return this;
        return this with { Stock = Stock + quantity };
    }
}

public class ProductImage
{
    public string Url { get; private set; } = default!;
    public string Alt { get; private set; } = default!;
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }

    private ProductImage() { }

    public static ProductImage Create(string url, string alt, bool isPrimary = false, int sortOrder = 0)
    {
        return new ProductImage
        {
            Url = url,
            Alt = alt,
            IsPrimary = isPrimary,
            SortOrder = sortOrder
        };
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
    }

    public void SetAsNonPrimary()
    {
        IsPrimary = false;
    }

    public void UpdateSortOrder(int order)
    {
        SortOrder = order;
    }
}

public class ProductVariant
{
    public string Name { get; private set; } = default!;
    public string Value { get; private set; } = default!;
    public Money? AdditionalPrice { get; private set; }
    public int? Stock { get; private set; }
    public string? SKU { get; private set; }

    private ProductVariant() { }

    public static ProductVariant Create(string name, string value, Money? additionalPrice = null, int? stock = null, string? sku = null)
    {
        return new ProductVariant
        {
            Name = name,
            Value = value,
            AdditionalPrice = additionalPrice,
            Stock = stock,
            SKU = sku
        };
    }

    public void UpdatePrice(Money? price)
    {
        AdditionalPrice = price;
    }

    public void UpdateStock(int? stock)
    {
        Stock = stock;
    }
}

public record ProductSpecification(string Name, string Value, string? Group = null);

public record SEO(string? MetaTitle, string? MetaDescription);

public class ProductRating
{
    public decimal AverageRating { get; private set; } = 0;
    public int ReviewCount { get; private set; } = 0;

    public void UpdateRating(decimal averageRating, int reviewCount)
    {
        AverageRating = Math.Round(averageRating, 2);
        ReviewCount = Math.Max(0, reviewCount);
    }

    public void AddReview(decimal rating)
    {
        var totalRating = AverageRating * ReviewCount + rating;
        ReviewCount++;
        AverageRating = Math.Round(totalRating / ReviewCount, 2);
    }
}

public class ProductAnalytics
{
    public int Views { get; private set; } = 0;
    public int Sales { get; private set; } = 0;
    public decimal Revenue { get; private set; } = 0;

    public void IncrementViews()
    {
        Views++;
    }

    public void RecordSale(int quantity = 1, decimal? revenue = null)
    {
        Sales += quantity;
        if (revenue.HasValue)
        {
            Revenue += revenue.Value;
        }
    }

    public decimal GetConversionRate()
    {
        return Views > 0 ? (decimal)Sales / Views * 100 : 0;
    }
}









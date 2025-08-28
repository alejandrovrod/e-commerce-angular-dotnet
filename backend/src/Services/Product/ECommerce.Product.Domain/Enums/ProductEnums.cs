namespace ECommerce.Product.Domain.Enums;

public enum ProductStatus
{
    Draft = 1,
    Active = 2,
    Inactive = 3,
    OutOfStock = 4,
    Discontinued = 5
}

public enum ProductType
{
    Physical = 1,
    Digital = 2,
    Service = 3,
    Subscription = 4
}

public enum InventoryStatus
{
    InStock = 1,
    LowStock = 2,
    OutOfStock = 3,
    Backorder = 4,
    Discontinued = 5
}

public enum SortBy
{
    Name = 1,
    Price = 2,
    CreatedDate = 3,
    Rating = 4,
    Sales = 5,
    Views = 6
}

public enum SortDirection
{
    Ascending = 1,
    Descending = 2
}






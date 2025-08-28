using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.GetProducts;

public class GetProductsQuery : IRequest<ApiResponse<PagedResult<ProductDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsDigital { get; set; }
    public string? SortBy { get; set; } = "name";
    public string? SortOrder { get; set; } = "asc";
    public List<string>? Tags { get; set; }
    public Domain.Enums.ProductStatus? Status { get; set; }
}

public class GetProductByIdQuery : IRequest<ApiResponse<ProductDto>>
{
    public Guid Id { get; }

    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetProductsByCategoryQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public Guid CategoryId { get; }
    public int Page { get; }
    public int PageSize { get; }

    public GetProductsByCategoryQuery(Guid categoryId, int page, int pageSize)
    {
        CategoryId = categoryId;
        Page = page;
        PageSize = pageSize;
    }
}

public class GetProductsByBrandQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public Guid BrandId { get; }
    public int Page { get; }
    public int PageSize { get; }

    public GetProductsByBrandQuery(Guid brandId, int page, int pageSize)
    {
        BrandId = brandId;
        Page = page;
        PageSize = pageSize;
    }
}

public class GetFeaturedProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public int Count { get; }

    public GetFeaturedProductsQuery(int count)
    {
        Count = count;
    }
}

public class GetRelatedProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public Guid ProductId { get; }
    public int Count { get; }

    public GetRelatedProductsQuery(Guid productId, int count)
    {
        ProductId = productId;
        Count = count;
    }
}

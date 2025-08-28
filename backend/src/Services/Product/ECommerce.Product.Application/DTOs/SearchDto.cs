namespace ECommerce.Product.Application.DTOs;

public class AdvancedSearchResultDto
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public SearchFacetsDto Facets { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public TimeSpan SearchTime { get; set; }
}

public class SearchFacetsDto
{
    public List<CategoryFacetDto> Categories { get; set; } = new();
    public List<BrandFacetDto> Brands { get; set; } = new();
    public List<PriceRangeFacetDto> PriceRanges { get; set; } = new();
    public List<TagFacetDto> Tags { get; set; } = new();
    public List<AttributeFacetDto> Attributes { get; set; } = new();
}

public class CategoryFacetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

public class BrandFacetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

public class PriceRangeFacetDto
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int Count { get; set; }
    public bool IsSelected { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class TagFacetDto
{
    public string Tag { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

public class AttributeFacetDto
{
    public string Name { get; set; } = string.Empty;
    public List<AttributeValueFacetDto> Values { get; set; } = new();
}

public class AttributeValueFacetDto
{
    public string Value { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

public class SearchResultDto
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public string Query { get; set; } = string.Empty;
    public TimeSpan SearchTime { get; set; }
}

public class VoiceSearchRequestDto
{
    public byte[] AudioData { get; set; } = Array.Empty<byte>();
    public string Language { get; set; } = "en-US";
}

public class SearchSuggestionsDto
{
    public string OriginalQuery { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = new();
    public List<string> DidYouMean { get; set; } = new();
    public List<string> PopularSearches { get; set; } = new();
}

public class PriceRangeDto
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int ProductCount { get; set; }
    public string Label { get; set; } = string.Empty;
}

public class ProductAttributeDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Values { get; set; } = new();
    public string Type { get; set; } = string.Empty; // text, number, boolean, select
}

public class TrendingProductDto
{
    public ProductDto Product { get; set; } = new();
    public int TrendScore { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // up, down, stable
    public decimal GrowthPercentage { get; set; }
}

public class SearchHistoryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Query { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }
    public string? Category { get; set; }
    public bool WasSuccessful { get; set; }
}


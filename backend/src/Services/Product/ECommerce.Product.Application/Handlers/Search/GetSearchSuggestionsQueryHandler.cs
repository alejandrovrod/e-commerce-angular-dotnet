using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.Queries.Search;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Search;

public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQuery, ApiResponse<List<string>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetSearchSuggestionsQueryHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<string>>> Handle(GetSearchSuggestionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Query) || request.Query.Length < 2)
                return ApiResponse<List<string>>.SuccessResult(new List<string>());

            var suggestions = new List<string>();
            
            // Get product name suggestions
            var products = await _productRepository.SearchAsync(request.Query, 1, 100);
            suggestions.AddRange(products.Take(5).Select(p => p.Name));

            // Get category suggestions
            var categories = await _categoryRepository.GetAllAsync();
            var matchingCategories = categories.Where(c => c.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            suggestions.AddRange(matchingCategories.Take(3).Select(c => c.Name));

            // Get brand suggestions
            var brands = await _brandRepository.GetAllAsync();
            var matchingBrands = brands.Where(b => b.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            suggestions.AddRange(matchingBrands.Take(3).Select(b => b.Name));

            var uniqueSuggestions = suggestions.Distinct().Take(10).ToList();
            return ApiResponse<List<string>>.SuccessResult(uniqueSuggestions);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<string>>.ErrorResult($"Suggestions error: {ex.Message}");
        }
    }
}

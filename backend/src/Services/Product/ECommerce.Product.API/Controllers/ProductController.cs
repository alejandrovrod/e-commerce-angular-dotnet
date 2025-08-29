using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.CreateProduct;
using ECommerce.Product.Application.Queries.GetProducts;
using ECommerce.Product.Domain.Repositories;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProductRepository _productRepository;

    public ProductController(IMediator mediator, IProductRepository productRepository)
    {
        _mediator = mediator;
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAll([FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<ProductDto>.ErrorResult("Product not found"));

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Price = product.Price.Amount,
            Brand = product.Brand,
            CategoryId = product.CategoryId,
            Status = product.Status,
            IsFeatured = product.IsFeatured,
            IsDigital = product.IsDigital,
            RequiresShipping = product.RequiresShipping,
            IsTaxable = product.IsTaxable,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Ok(ApiResponse<ProductDto>.SuccessResult(productDto));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetBySlug(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug);
        if (product == null)
            return NotFound(ApiResponse<ProductDto>.ErrorResult("Product not found"));

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Price = product.Price.Amount,
            Brand = product.Brand,
            CategoryId = product.CategoryId,
            Status = product.Status,
            IsFeatured = product.IsFeatured,
            IsDigital = product.IsDigital,
            RequiresShipping = product.RequiresShipping,
            IsTaxable = product.IsTaxable,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Ok(ApiResponse<ProductDto>.SuccessResult(productDto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(Guid id, [FromBody] CreateProductCommand command)
    {
        // TODO: Implement UpdateProductCommand
        return BadRequest(ApiResponse<ProductDto>.ErrorResult("Update not implemented yet"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<bool>.ErrorResult("Product not found"));

        await _productRepository.DeleteAsync(product);
        return Ok(ApiResponse<bool>.SuccessResult(true));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] string status)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<bool>.ErrorResult("Product not found"));

        // TODO: Implement status update logic
        return Ok(ApiResponse<bool>.SuccessResult(true));
    }

    [HttpPatch("{id}/featured")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleFeatured(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<bool>.ErrorResult("Product not found"));

        // TODO: Implement featured toggle logic
        return Ok(ApiResponse<bool>.SuccessResult(true));
    }

    [HttpGet("debug")]
    public async Task<ActionResult<object>> DebugProducts()
    {
        try
        {
            var productsFromContext = await _productRepository.GetAllAsync();
            
            var debugInfo = new
            {
                ContextProductsCount = productsFromContext.Count(),
                ContextProducts = productsFromContext.Select(p => new { p.Id, p.Name, p.SKU, p.Status, p.IsFeatured, p.CreatedAt }).ToList(),
                Message = "Debug info from ProductController"
            };
            
            return Ok(debugInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}

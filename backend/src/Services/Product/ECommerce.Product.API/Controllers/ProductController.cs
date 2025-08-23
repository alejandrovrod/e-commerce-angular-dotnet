using Microsoft.AspNetCore.Mvc;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands;
using ECommerce.Product.Application.Commands.CreateProduct;
using ECommerce.Product.Application.Queries;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMediator mediator, ILogger<ProductController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los productos con paginación y filtros
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] string? brand = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? inStock = null)
    {
        try
        {
            var query = new GetProductsQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Category = category,
                Brand = brand,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                InStock = inStock
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Obtiene un producto por su ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { error = "Product not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Obtiene un producto por su slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug)
    {
        try
        {
            var query = new GetProductBySlugQuery { Slug = slug };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { error = "Product not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by slug {Slug}", slug);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result.Data);
            }
            
            return BadRequest(new { error = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound(new { error = "Product not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Elimina un producto
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { error = "Product not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Publica un producto (cambia estado de Draft a Active)
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    public async Task<ActionResult<ProductDto>> PublishProduct(Guid id)
    {
        try
        {
            var command = new PublishProductCommand { Id = id };
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound(new { error = "Product not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing product {ProductId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Actualiza el inventario de un producto
    /// </summary>
    [HttpPut("{id:guid}/inventory")]
    public async Task<ActionResult<ProductDto>> UpdateInventory(Guid id, [FromBody] UpdateInventoryCommand command)
    {
        try
        {
            command.ProductId = id;
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound(new { error = "Product not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory for product {ProductId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Obtiene productos destacados
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts([FromQuery] int limit = 10)
    {
        try
        {
            var query = new GetFeaturedProductsQuery { Limit = limit };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured products");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Busca productos por texto
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> SearchProducts(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new SearchProductsQuery
            {
                SearchTerm = q,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", q);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}


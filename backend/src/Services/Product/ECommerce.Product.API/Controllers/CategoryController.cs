using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.Category;
using ECommerce.Product.Application.Queries.Category;
using ECommerce.Product.Domain.Repositories;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryController(IMediator mediator, ICategoryRepository categoryRepository)
    {
        _mediator = mediator;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll([FromQuery] GetCategoriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id)
    {
        var query = new GetCategoryByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}/subcategories")]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetSubcategories(Guid id)
    {
        var query = new GetSubcategoriesQuery { ParentCategoryId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("debug")]
    public async Task<ActionResult<object>> DebugCategories()
    {
        try
        {
            // Obtener categorías del contexto
            var categoriesFromContext = await _categoryRepository.GetAllAsync();
            
            var debugInfo = new
            {
                ContextCategoriesCount = categoriesFromContext.Count,
                ContextCategories = categoriesFromContext.Select(c => new { c.Id, c.Name, c.IsActive, c.CreatedAt }),
                Message = "Debug info from CategoryController"
            };
            
            return Ok(debugInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}

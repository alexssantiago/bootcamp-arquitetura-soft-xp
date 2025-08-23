using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Interfaces;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Requests;
using XPE.ArquiteturaSoftware.DesafioFinal.Application.Responses;

namespace XPE.ArquiteturaSoftware.DesafioFinal.Api.Controllers;

/// <summary>
/// Product endpoints for creating, reading, updating and deleting products.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    /// <summary>Creates a new product.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create product")]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status200OK)]
    public Task<Result<CreateProductResponse>> CreateAsync([FromBody] CreateProductRequest request, CancellationToken ct)
        => service.CreateAsync(request);

    /// <summary>Gets the total number of products.</summary>
    [HttpGet("count")]
    [SwaggerOperation(Summary = "Count products")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public Task<Result<int>> CountAsync(CancellationToken ct)
        => service.CountAsync();

    /// <summary>Returns all products.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Find all products")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public Task<Result<IEnumerable<ProductResponse>>> GetAllAsync(CancellationToken ct)
        => service.GetAllAsync();

    /// <summary>Gets a product by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Find product by id")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    public Task<Result<ProductResponse>> GetByIdAsync(int id, CancellationToken ct)
        => service.GetByIdAsync(id);

    /// <summary>Finds products by name (prefix match).</summary>
    [HttpGet("by-name")]
    [SwaggerOperation(Summary = "Find products by name")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public Task<Result<IEnumerable<ProductResponse>>> FindByNameAsync([FromQuery] string name, CancellationToken ct)
        => service.FindByNameAsync(name);

    /// <summary>Updates a product by id.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<Result> UpdateAsync(int id, [FromBody] UpdateProductRequest request, CancellationToken ct)
        => service.UpdateAsync(id, request);

    /// <summary>Deletes a product by id.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<Result> DeleteAsync(int id, CancellationToken ct)
        => service.DeleteAsync(id);
}
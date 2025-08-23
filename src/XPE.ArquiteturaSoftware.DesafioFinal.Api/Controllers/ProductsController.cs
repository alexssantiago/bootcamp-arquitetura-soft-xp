using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using XPE.ArquiteturaSoftware.DesafioFinal.Api.Extensions;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProductRequest request, CancellationToken ct)
        => (await service.CreateAsync(request)).ToActionResult(this);

    /// <summary>Gets the total number of products.</summary>
    [HttpGet("count")]
    [SwaggerOperation(Summary = "Count products")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> CountAsync(CancellationToken ct)
        => (await service.CountAsync()).ToActionResult(this);

    /// <summary>Returns all products.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Find all products")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        => (await service.GetAllAsync()).ToActionResult(this);

    /// <summary>Gets a product by id.</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Find product by id")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
        => (await service.GetByIdAsync(id)).ToActionResult(this);

    /// <summary>Finds products by name (prefix match).</summary>
    [HttpGet("by-name")]
    [SwaggerOperation(Summary = "Find products by name")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> FindByNameAsync([FromQuery] string name, CancellationToken ct)
        => (await service.FindByNameAsync(name)).ToActionResult(this);

    /// <summary>Updates a product by id.</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProductRequest request, CancellationToken ct)
        => (await service.UpdateAsync(id, request)).ToActionResult(this);

    /// <summary>Deletes a product by id.</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete product")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct)
        => (await service.DeleteAsync(id)).ToActionResult(this);
}

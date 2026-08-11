using Microsoft.AspNetCore.Mvc;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Interfaces;

namespace ScaleUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: /api/category
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsDtoAsync();
        return Ok(categories);
    }

    // GET: /api/category/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsDtoAsync(id);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    // POST: /api/category
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _categoryService.CreateFromRequestAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE: /api/category/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        _categoryService.Remove(category);
        await _categoryService.SaveChangesAsync();

        return NoContent();
    }
}

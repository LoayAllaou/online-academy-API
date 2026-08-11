using Microsoft.AspNetCore.Mvc;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Interfaces;

namespace ScaleUp.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class InstructorController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    // GET: api/instructor/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InstructorDto>> Get(int id)
    {
        var instructor = await _instructorService.GetInstructorByIdAsync(id);
        if (instructor == null) return NotFound();

        return Ok(instructor);
    }

    // POST: api/instructor
    [HttpPost]
    public async Task<ActionResult<InstructorDto>> Create([FromBody] InstructorCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _instructorService.CreateInstructorAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT: api/instructor/5
    [HttpPut("{id}")]
    public async Task<ActionResult<InstructorDto>> Update(int id, [FromBody] InstructorCreateUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _instructorService.UpdateInstructorAsync(id, dto);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    // GET: /api/instructor
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstructorDto>>> GetAll()
    {
        var instructors = await _instructorService.GetAllInstructorsAsync();
        if (instructors == null || !instructors.Any()) return NotFound();

        return Ok(instructors);
    }
}

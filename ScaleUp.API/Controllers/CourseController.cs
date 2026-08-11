using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Interfaces;

namespace ScaleUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IMapper _mapper;

    public CourseController(ICourseService courseService, IMapper mapper)
    {
        _courseService = courseService;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseDetails(int id)
    {
        var course = await _courseService.GetCourseWithDetailsAsync(id);
        if (course == null) return NotFound();

        var courseDto = _mapper.Map<CourseDto>(course);
        return Ok(courseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        var courses = await _courseService.GetAllCoursesWithDetailsAsync();
        var courseDtos = _mapper.Map<List<CourseDto>>(courses);
        foreach (var courseDto in courseDtos)
        {
            courseDto.TotalLessons = courseDto.Titles.Count == 0 ? 0 : courseDto.Titles.Sum(t => t.Videos.Count);
        }
        return Ok(courseDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdCourse = await _courseService.CreateCourseAsync(dto);
        var courseDto = _mapper.Map<CourseDto>(createdCourse);

        return CreatedAtAction(nameof(GetCourseDetails), new { id = courseDto.Id }, courseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseCreateUpdateDto dto)
    {
        var updatedCourse = await _courseService.UpdateCourseAsync(id, dto);
        if (updatedCourse == null)
            return NotFound();

        var courseDto = _mapper.Map<CourseDto>(updatedCourse);
        return Ok(courseDto);
    }

    // PSEUDOCODE / PLAN (detailed):
    // 1. Validate input: if videos list is null or empty -> return BadRequest.
    // 2. Prepare list to collect saved file relative paths to return to caller.
    // 3. Determine a safe upload directory under wwwroot/UploadedVideos/{title}.
    //    - Use Directory.GetCurrentDirectory() to build an absolute path.
    //    - Ensure the directory exists with Directory.CreateDirectory(...).
    // 4. For each uploaded IFormFile:
    //    - Skip files with zero length.
    //    - Sanitize file name with Path.GetFileName(...) to avoid directory traversal.
    //    - Optionally create a unique file name to avoid collisions (append GUID).
    //    - Combine upload directory and file name to get absolute file path.
    //    - Open a FileStream with FileMode.Create and copy the uploaded file stream to it asynchronously.
    //    - Build a client-friendly relative URL/path (e.g., "/UploadedVideos/{title}/{filename}") and add to list.
    // 5. After processing all files, return 200 OK with a JSON payload containing a message and the list of saved paths.
    // 6. Keep error handling minimal here; let framework handle exceptions (could be extended to handle IO errors).
    //Allow only videos extenstions

    [HttpPost("upload-section")]
    [RequestSizeLimit(1_000_000_000)] // 1GB example
    public async Task<IActionResult> UploadTitleVideos(
        [FromForm] string title,
        [FromForm] List<IFormFile> videos)
    {
        if (videos == null || videos.Count == 0)
            return BadRequest("No videos uploaded.");

            var savedPaths = new List<string>();

            // Build absolute upload root: <project-root>/wwwroot/UploadedVideos/{title}
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedVideos", string.IsNullOrWhiteSpace(title) ? "default" : title);
            Directory.CreateDirectory(uploadsRoot);

            foreach (var file in videos)
            {
                if (file == null || file.Length == 0)
                    continue;

                // Sanitize original file name
                var originalFileName = Path.GetFileName(file.FileName) ?? "upload";
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
                var extension = Path.GetExtension(originalFileName);

                // Create a unique file name to avoid collisions
                var uniqueFileName = $"{fileNameWithoutExt}_{Guid.NewGuid():N}{extension}";

                var absolutePath = Path.Combine(uploadsRoot, uniqueFileName);

                // Save file to disk
                await using (var fileStream = new FileStream(absolutePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Create a client-friendly relative path (use forward slashes)
                var relativePath = $"/UploadedVideos/{(string.IsNullOrWhiteSpace(title) ? "default" : title)}/{uniqueFileName}";
                savedPaths.Add(relativePath);
            }

            return Ok(new { Message = "Videos uploaded successfully.", Paths = savedPaths });
    }
}


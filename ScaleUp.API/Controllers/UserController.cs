using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using ScaleUp.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ScaleUp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // GET api/user/me
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var studentId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) // "sub"
             ?? User.FindFirstValue(ClaimTypes.NameIdentifier) // NameIdentifier
             ?? User.Identity?.Name; // fallback

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized("User not found in token");

        var user = await _userManager.FindByIdAsync(studentId);
        if (user == null) return NotFound("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new UserInfoDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Phone = user.PhoneNumber,
            Birthday = user.Birthday,
            Country = user.Country,
            ProfilePictureUrl = user.ProfilePictureUrl,
        };

        return Ok(dto);
    }

    // PUT api/user/update
    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoRequest model)
    {
        var studentId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) // "sub"
     ?? User.FindFirstValue(ClaimTypes.NameIdentifier) // NameIdentifier
     ?? User.Identity?.Name; // fallback

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized("User not found in token");

        var user = await _userManager.FindByIdAsync(studentId);
        if (user == null) return NotFound("User not found.");

        if (!string.IsNullOrWhiteSpace(model.FullName))
            user.FullName = model.FullName;

        if (model.Birthday.HasValue)
            user.Birthday = model.Birthday.Value;

        if (!string.IsNullOrWhiteSpace(model.Country))
            user.Country = model.Country;

        //if (!string.IsNullOrWhiteSpace(model.ProfilePictureUrl))
        //    user.ProfilePictureUrl = model.ProfilePictureUrl;

        if (!string.IsNullOrEmpty(model.Phone))
            user.PhoneNumber = model.Phone;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User info updated successfully.");
    }

    // PUT api/user/change-password
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Password changed successfully.");
    }

    [HttpPost("upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        // Ensure upload folder exists
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-pics");
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Build file URL
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var fileUrl = $"{baseUrl}/uploads/profile-pics/{fileName}";

        // Update user
        user.ProfilePictureUrl = fileUrl;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "Profile picture uploaded successfully.", Url = fileUrl });
    }


    [Authorize]
    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("User not found.");

        // Check role
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Student"))
        {
            // Courses student enrolled in
            var courses = await _context.Enrollments
                .Where(e => e.StudentId == user.Id)
                .Select(e => e.Course)
                .ToListAsync();

            return Ok(courses);
        }

        return Forbid();
    }

}

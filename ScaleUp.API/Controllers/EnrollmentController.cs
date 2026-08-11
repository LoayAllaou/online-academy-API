using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.DTOs.Enrollment;
using ScaleUp.Core.Entities;
using ScaleUp.Core.Enums;
using ScaleUp.Core.Helpers;
using ScaleUp.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ScaleUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public EnrollmentController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    //[HttpPost("add")]
    //public async Task<IActionResult> AddToCart(int courseId, string userId, string? couponCode)
    //{
    //    var exists = await _context.Enrollments
    //        .AnyAsync(e => e.CourseId == courseId && e.StudentId == userId && e.Status != EnrollmentStatus.Cancelled);

    //    if (exists)
    //        return BadRequest("Course already in cart or purchased.");

    //    // Check if course exists
    //    var course = await _context.Courses.FindAsync(courseId);
    //    if (course == null) return NotFound("Course not found");

    //    decimal discountAmount = 0;
    //    Coupon? coupon = null;

    //    // Apply coupon if provided
    //    if (!string.IsNullOrEmpty(couponCode))
    //    {
    //        coupon = await _context.Coupons
    //            .FirstOrDefaultAsync(c => c.Code == couponCode && c.IsActive &&
    //                c.StartDate <= DateTime.UtcNow && c.EndDate >= DateTime.UtcNow);

    //        if (coupon == null)
    //            return BadRequest("Invalid or expired coupon.");

    //        discountAmount = (course.Price * coupon.DiscountPercentage) / 100;
    //    }

    //    var enrollment = new Enrollment
    //    {
    //        CourseId = courseId,
    //        StudentId = userId,
    //        Status = EnrollmentStatus.Pending,
    //        Price = course.Price,
    //        DiscountAmount = discountAmount,
    //        CouponId = coupon?.Id
    //    };

    //    _context.Enrollments.Add(enrollment);
    //    await _context.SaveChangesAsync();

    //    return Ok(enrollment);
    //}

    [HttpPost("pay")]
    public async Task<IActionResult> MarkAsPaid(int enrollmentId)
    {
        var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
        if (enrollment == null) return NotFound();

        enrollment.Status = EnrollmentStatus.Paid;
        await _context.SaveChangesAsync();

        return Ok(enrollment);
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyEnrollments()
    {
        // Try multiple claim sources depending on how JWT was created
        var studentId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) // "sub"
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier) // NameIdentifier
                     ?? User.Identity?.Name; // fallback

        //var cartItems = await _context.Enrollments
        //.Include(e => e.Course)
        //.Where(e => e.StudentId == studentId)
        //.Select(e => new { e.CourseId, e.Course.TitleEn, e.Course.Price })
        //.ToListAsync();

        var list = await _context.Enrollments.Where(e => e.Status == EnrollmentStatus.Pending && e.StudentId == studentId)
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .ToListAsync();

        //map list to CartItemsDto
        if (list == null) return NotFound();

        var cartItems = _mapper.Map<List<CartItemsDto>>(list);

        return Ok(cartItems);
    }

    [HttpPost("add-to-cart")]
    [Authorize(Roles = "Student")] // optional: only students can add to cart
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        // Try multiple claim sources depending on how JWT was created
        var studentId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) // "sub"
                     ?? User.FindFirstValue(ClaimTypes.NameIdentifier) // NameIdentifier
                     ?? User.Identity?.Name; // fallback

        if (string.IsNullOrEmpty(studentId))
            return Unauthorized("User not found in token");

        // Validate course
        var course = await _context.Courses.FindAsync(dto.CourseId);
        if (course == null)
            return NotFound("Course not found.");

        // Check if already in cart
        var exists = await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == dto.CourseId && e.Status != EnrollmentStatus.Cancelled);

        if (exists)
            return BadRequest("Course already in cart.");

        // Validate coupon (if provided)
        Coupon? coupon = null;
        if (!string.IsNullOrEmpty(dto.CouponCode))
        {
            coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == dto.CouponCode);

            if (coupon == null || !coupon.IsValid())
                return BadRequest("Invalid or expired coupon.");
        }

        // Create enrollment
        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = dto.CourseId,
            CouponId = coupon?.Id,
            Status = EnrollmentStatus.Pending,
            Price = course.Price,
            DiscountAmount = coupon != null ? (course.Price * coupon.DiscountPercentage / 100) : 0,
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            enrollment.Id,
            enrollment.StudentId,
            enrollment.CourseId,
            CouponApplied = coupon?.Code
        });
    }

    [Authorize]
    [HttpGet("my-courses")]
    public async Task<IActionResult> GetMyCourses()
    {
        // Extract user info from JWT
        var userId =
            User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.Identity?.Name;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var userRole = User.FindFirstValue(ClaimTypes.Role);

        //IQueryable<Course> query;
        IQueryable<Course> query = _context.Courses.Where(c => false);

        // Handle by role
        if (userRole == UserRoles.Student)
        {
            // Student → Courses enrolled in
            query = _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Titles)
                        .ThenInclude(t => t.Videos)
                .Where(e => e.StudentId == userId)
                .Select(e => e.Course);
        }
        else if (userRole == UserRoles.Instructor)
        {
            // Instructor → Courses created by this instructor
            //query = _context.Courses
            //    .Include(c => c.Titles)
            //        .ThenInclude(t => t.Videos)
            //    .Where(c => c.InstructorId == userId);
        }
        else if (userRole == UserRoles.Admin)
        {
            // Admin → All courses
            query = _context.Courses
                .Include(c => c.Titles)
                    .ThenInclude(t => t.Videos);
        }
        else
        {
            return Forbid("Unauthorized role.");
        }

        var myCourses = await query.AsNoTracking().ToListAsync();

        if (myCourses == null || !myCourses.Any())
            return NotFound("No courses found for this user.");

        var courseDtos = _mapper.Map<List<CourseDto>>(myCourses);

        return Ok(courseDtos);
    }

    // Fixing the method signature to resolve the errors
    [HttpPost("place-order")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MarkAsPendingPayment([FromBody] PlaceOrderDto dto)
    {
        // Iterate through the provided enrollment IDs and update their status
        foreach (var enrollmentId in dto.EnrollmentIds)
        {
            var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
            if (enrollment == null) return NotFound($"Enrollment with ID {enrollmentId} not found.");

            enrollment.Status = EnrollmentStatus.PendingPayment;
        }

        await _context.SaveChangesAsync();
        return Ok("Enrollments updated to Pending Payment status.");
    }
}

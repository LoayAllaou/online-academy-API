using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using ScaleUp.Infrastructure.Data;

namespace ScaleUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CouponController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/coupon
    [HttpGet]
    public IActionResult GetAllCoupons()
    {
        var coupons = _context.Coupons.ToList();
        return Ok(coupons);
    }

    // GET: api/coupon/{id}
    [HttpGet("{id}")]
    public IActionResult GetCouponById(int id)
    {
        var coupon = _context.Coupons.Find(id);
        if (coupon == null)
        {
            return NotFound();
        }
        return Ok(coupon);
    }

    /// <summary>
    /// Add a new coupon
    /// </summary>
    [HttpPost("add")]
    public async Task<IActionResult> AddCoupon([FromBody] CouponDto dto)
    {
        // Check if coupon code already exists
        var exists = await _context.Coupons.AnyAsync(c => c.Code == dto.Code);
        if (exists)
            return BadRequest("Coupon code already exists.");

        var coupon = new Coupon
        {
            Code = dto.Code,
            DiscountPercentage = dto.DiscountPercentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            coupon.Id,
            coupon.Code,
            coupon.DiscountPercentage,
            coupon.StartDate,
            coupon.EndDate,
            coupon.IsActive
        });
    }

    // Update existing coupon
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCoupon(int id, [FromBody] Coupon updatedCoupon)
    {
        var existingCoupon = await _context.Coupons.FindAsync(id);
        if (existingCoupon == null)
        {
            return NotFound();
        }
        existingCoupon.Code = updatedCoupon.Code;
        existingCoupon.DiscountPercentage = updatedCoupon.DiscountPercentage;
        existingCoupon.EndDate = updatedCoupon.EndDate;
        _context.Coupons.Update(existingCoupon);
        await _context.SaveChangesAsync();
        return Ok(existingCoupon);
    }

    // Validate coupon
    [HttpGet("validate/{code}")]
    public async Task<IActionResult> ValidateCoupon(string code)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive);

        if (coupon == null) return NotFound("Coupon not found or inactive.");

        if (DateTime.UtcNow < coupon.StartDate || DateTime.UtcNow > coupon.EndDate)
            return BadRequest("Coupon is expired or not yet valid.");

        return Ok(coupon);
    }
}

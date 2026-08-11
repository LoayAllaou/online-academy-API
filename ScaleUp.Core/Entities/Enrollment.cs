using ScaleUp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class Enrollment : BaseEntity
{
    public string StudentId { get; set; } = null!;   // UserId is string
    public ApplicationUser Student { get; set; } = null!;
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public decimal Price { get; set; }       // Original price
    public decimal DiscountAmount { get; set; } = 0; // Discount applied
    public int? CouponId { get; set; }       // Optional
    public Coupon? Coupon { get; set; }
}

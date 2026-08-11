using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class AddToCartDto
{
    public int CourseId { get; set; }
    public string? CouponCode { get; set; } // Optional
}

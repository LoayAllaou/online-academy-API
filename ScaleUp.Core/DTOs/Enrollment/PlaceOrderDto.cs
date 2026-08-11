using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs.Enrollment;

public class PlaceOrderDto
{
    public int[] EnrollmentIds { get; set; }
    public string? CouponCode { get; set; } // Optional
}

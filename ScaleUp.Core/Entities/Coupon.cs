using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = null!;      // e.g., "SUMMER50"
    public decimal DiscountPercentage { get; set; } // e.g., 10, 25, 50
    public DateTime StartDate { get; set; }         // Valid from
    public DateTime EndDate { get; set; }           // Valid until
    // Optional: limit usage per user
    public int? MaxUsagePerUser { get; set; }

    public bool IsValid()
    {
        return DateTime.UtcNow <= EndDate;
    }
}

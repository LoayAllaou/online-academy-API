using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Enums;

public enum EnrollmentStatus
{
    Pending,   // Added to cart, not paid
    PendingPayment, //Waiting for payment
    Paid,      // Paid and active
    Cancelled  // Optional: removed or refunded
}

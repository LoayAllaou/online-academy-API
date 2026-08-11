using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; } // Primary key for the entity
    public string CreatedBy { get; set; } // Optional, can be used to track who created the entity
    public string? UpdatedBy { get; set; } // Optional, can be used to track who last updated the entity
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true; // Indicates if the entity is active
    public bool IsDeleted { get; set; } = false;
    // Optionally, you can add a method to mark the entity as deleted
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

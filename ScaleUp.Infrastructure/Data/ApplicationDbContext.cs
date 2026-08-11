using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScaleUp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IHttpContextAccessor httpContextAccessor)
        : base(options) 
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseTitle> CourseTitles { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // ⬅️ VERY IMPORTANT for Identity setup

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Courses)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Titles)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId);

        modelBuilder.Entity<CourseTitle>()
            .HasMany(t => t.Videos)
            .WithOne(v => v.CourseTitle)
            .HasForeignKey(v => v.CourseTitleId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Coupon)
            .WithMany()
            .HasForeignKey(e => e.CouponId)
            .IsRequired(false);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "System";

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userName;
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.IsActive = true;
                entry.Entity.IsDeleted = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedBy = userName;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

}

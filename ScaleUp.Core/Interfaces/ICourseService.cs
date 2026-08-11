using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Interfaces;

public interface ICourseService : IRepository<Course>
{
    Task<Course> GetCourseWithDetailsAsync(int courseId);
    Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync();
    Task<Course> CreateCourseAsync(CourseCreateUpdateDto dto);
    Task<Course> UpdateCourseAsync(int id, CourseCreateUpdateDto dto);
}

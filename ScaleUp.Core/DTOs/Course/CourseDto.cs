using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string DescriptionEn { get; set; }
    public string DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; }
    public List<CourseTitleDto> Titles { get; set; }
    public CategoryDto Category { get; set; }
    public InstructorDto Instructor { get; set; }
    public double AverageRating { get; set; } = 4.3;
    public int TotalRatings { get; set; } = 1200;
    public bool IsPaid { get; set; } = true;
    public int TotalLessons { get; set; }
    public int TotalDurationInMinutes { get; set; } = 1000;// Total duration of all videos in minutes
}

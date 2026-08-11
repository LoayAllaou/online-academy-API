using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class CourseCreateUpdateDto
{
    public int? Id { get; set; }  // Null for new courses
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string DescriptionEn { get; set; }
    public string DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; }
    public int CategoryId { get; set; }
    public int InstructorId { get; set; }
    public List<CourseTitleCreateUpdateDto> Titles { get; set; }
}
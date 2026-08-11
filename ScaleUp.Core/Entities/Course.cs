using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class Course : BaseEntity
{
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public string DescriptionEn { get; set; }
    public string DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; }

    public List<CourseTitle> Titles { get; set; }
}

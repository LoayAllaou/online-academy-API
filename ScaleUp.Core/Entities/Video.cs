using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class Video : BaseEntity
{
    public string Title { get; set; }
    public string VideoUrl { get; set; }

    public int CourseTitleId { get; set; }
    public CourseTitle CourseTitle { get; set; }
}

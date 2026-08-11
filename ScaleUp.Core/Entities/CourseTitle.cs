using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class CourseTitle : BaseEntity
{
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }

    public List<Video> Videos { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class Instructor : BaseEntity
{
    public string FullName { get; set; }
    public string Bio { get; set; }
    public string ProfileImageUrl { get; set; }

    public List<Course> Courses { get; set; }
}
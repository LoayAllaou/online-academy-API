using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Entities;

public class Category : BaseEntity
{
    public string NameEn { get; set; }
    public string NameAr { get; set; }

    public List<Course> Courses { get; set; }
}

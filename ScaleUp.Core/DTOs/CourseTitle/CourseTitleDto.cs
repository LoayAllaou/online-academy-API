using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class CourseTitleDto
{
    public int Id { get; set; }
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public List<VideoDto> Videos { get; set; }
}

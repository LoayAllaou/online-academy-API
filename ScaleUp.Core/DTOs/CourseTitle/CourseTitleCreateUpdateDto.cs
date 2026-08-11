using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class CourseTitleCreateUpdateDto
{
    public int? Id { get; set; }  // Null for new titles
    public string TitleEn { get; set; }
    public string TitleAr { get; set; }
    public List<VideoCreateUpdateDto> Videos { get; set; }
}

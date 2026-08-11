using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class VideoCreateUpdateDto
{
    public int? Id { get; set; }  // Null for new videos
    public string Title { get; set; }
    public string VideoUrl { get; set; }
}
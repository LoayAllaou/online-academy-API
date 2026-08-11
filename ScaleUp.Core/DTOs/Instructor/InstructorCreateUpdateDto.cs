using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class InstructorCreateUpdateDto
{
    public string FullName { get; set; }
    public string Bio { get; set; }
    public string ProfileImageUrl { get; set; }
}

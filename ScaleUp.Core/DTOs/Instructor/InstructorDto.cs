using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.DTOs;

public class InstructorDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Bio { get; set; }
    public string ProfileImageUrl { get; set; }
}

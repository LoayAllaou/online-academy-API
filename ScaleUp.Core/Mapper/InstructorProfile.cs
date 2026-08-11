using AutoMapper;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Mapper;

public class InstructorProfile : Profile
{
    public InstructorProfile()
    {
        CreateMap<Instructor, InstructorDto>().ReverseMap();
        CreateMap<InstructorCreateUpdateDto, Instructor>();
    }
}

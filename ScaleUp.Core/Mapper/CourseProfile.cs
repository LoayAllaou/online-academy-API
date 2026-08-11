using AutoMapper;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
namespace ScaleUp.Core.Mapper;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Video, VideoDto>();
        CreateMap<CourseTitle, CourseTitleDto>();
        CreateMap<Category, CategoryDto>();
        CreateMap<Instructor, InstructorDto>();
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor));

        CreateMap<VideoCreateUpdateDto, Video>()
    .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue));

        CreateMap<CourseTitleCreateUpdateDto, CourseTitle>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue))
            .ForMember(dest => dest.Videos, opt => opt.MapFrom(src => src.Videos));

        CreateMap<CourseCreateUpdateDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue))
            .ForMember(dest => dest.Titles, opt => opt.MapFrom(src => src.Titles));

        CreateMap<Enrollment, CartItemsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}

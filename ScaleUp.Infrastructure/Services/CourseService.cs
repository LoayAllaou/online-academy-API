using Microsoft.EntityFrameworkCore;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using ScaleUp.Core.Interfaces;
using ScaleUp.Infrastructure.Data;
using ScaleUp.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace ScaleUp.Infrastructure.Services;

public class CourseService : Repository<Course>, ICourseService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CourseService(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Course> GetCourseWithDetailsAsync(int courseId)
    {
        return await _context.Courses
            .Include(c => c.Titles)
                .ThenInclude(t => t.Videos)
                .Include(a => a.Category)
                 .Include(i => i.Instructor)
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }

    public async Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync()
    {
        return await _context.Courses
            .Include(c => c.Titles)
                .ThenInclude(t => t.Videos)
                .Include(a => a.Category)
                .Include(i => i.Instructor)
            .ToListAsync();
    }

    public async Task<Course> CreateCourseAsync(CourseCreateUpdateDto dto)
    {
        var course = _mapper.Map<Course>(dto);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<Course> UpdateCourseAsync(int id, CourseCreateUpdateDto dto)
    {
        var course = await _context.Courses
            .Include(c => c.Titles)
                .ThenInclude(t => t.Videos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return null;

        // Map simple props
        _mapper.Map(dto, course);

        // Update Titles & Videos manually to handle adds/updates/deletes

        // Handle CourseTitles
        foreach (var titleDto in dto.Titles)
        {
            var existingTitle = course.Titles.FirstOrDefault(t => t.Id == titleDto.Id);

            if (existingTitle == null)
            {
                // New title
                var newTitle = _mapper.Map<CourseTitle>(titleDto);
                course.Titles.Add(newTitle);
            }
            else
            {
                // Update existing title
                _mapper.Map(titleDto, existingTitle);

                // Handle Videos for this title
                foreach (var videoDto in titleDto.Videos)
                {
                    var existingVideo = existingTitle.Videos.FirstOrDefault(v => v.Id == videoDto.Id);

                    if (existingVideo == null)
                    {
                        var newVideo = _mapper.Map<Video>(videoDto);
                        existingTitle.Videos.Add(newVideo);
                    }
                    else
                    {
                        _mapper.Map(videoDto, existingVideo);
                    }
                }

                // Remove videos not in dto
                var videoIdsDto = titleDto.Videos.Where(v => v.Id.HasValue).Select(v => v.Id.Value).ToList();
                var videosToRemove = existingTitle.Videos.Where(v => !videoIdsDto.Contains(v.Id)).ToList();
                foreach (var v in videosToRemove)
                    existingTitle.Videos.Remove(v);
            }
        }

        // Remove titles not in dto
        var titleIdsDto = dto.Titles.Where(t => t.Id.HasValue).Select(t => t.Id.Value).ToList();
        var titlesToRemove = course.Titles.Where(t => !titleIdsDto.Contains(t.Id)).ToList();
        foreach (var t in titlesToRemove)
            course.Titles.Remove(t);

        await _context.SaveChangesAsync();

        return course;
    }
}

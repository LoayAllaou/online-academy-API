using AutoMapper;
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

namespace ScaleUp.Infrastructure.Services;

public class InstructorService : Repository<Instructor>, IInstructorService
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public InstructorService(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InstructorDto> GetInstructorByIdAsync(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null) return null;

        return _mapper.Map<InstructorDto>(instructor);
    }

    public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
    {
        var instructors = await _context.Instructors.ToListAsync();
        return _mapper.Map<IEnumerable<InstructorDto>>(instructors);
    }

    public async Task<InstructorDto> CreateInstructorAsync(InstructorCreateUpdateDto dto)
    {
        var instructor = _mapper.Map<Instructor>(dto);
        _context.Instructors.Add(instructor);
        await _context.SaveChangesAsync();

        return _mapper.Map<InstructorDto>(instructor);
    }

    public async Task<InstructorDto> UpdateInstructorAsync(int id, InstructorCreateUpdateDto dto)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor == null) return null;

        _mapper.Map(dto, instructor);
        await _context.SaveChangesAsync();

        return _mapper.Map<InstructorDto>(instructor);
    }
}

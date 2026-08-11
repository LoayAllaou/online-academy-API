using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Interfaces;

public interface IInstructorService : IRepository<Instructor>
{
    Task<InstructorDto> GetInstructorByIdAsync(int id);
    Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync();
    Task<InstructorDto> CreateInstructorAsync(InstructorCreateUpdateDto dto);
    Task<InstructorDto> UpdateInstructorAsync(int id, InstructorCreateUpdateDto dto);
}

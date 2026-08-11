using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Core.Interfaces;

public interface ICategoryService : IRepository<Category>
{
    // Additional category-specific methods can go here if needed
    Task<List<CategoryDto>> GetAllAsDtoAsync();
    Task<CategoryDto?> GetByIdAsDtoAsync(int id);
    Task<CategoryDto> CreateFromRequestAsync(CreateCategoryRequest request);
}

using Microsoft.EntityFrameworkCore;
using ScaleUp.Core.DTOs;
using ScaleUp.Core.Entities;
using ScaleUp.Core.Interfaces;
using ScaleUp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Category> _dbSet;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Category>();
    }

    public async Task<List<Category>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task<List<Category>> FindAsync(Expression<Func<Category, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(Category entity)
        => await _dbSet.AddAsync(entity);

    public void Remove(Category entity)
        => _dbSet.Remove(entity);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    // Category-specific methods
    public async Task<List<CategoryDto>> GetAllAsDtoAsync()
    {
        return await _dbSet
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn,
                NameAr = c.NameAr
            }).ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsDtoAsync(int id)
    {
        var category = await _dbSet.FindAsync(id);
        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            NameEn = category.NameEn,
            NameAr = category.NameAr
        };
    }

    public async Task<CategoryDto> CreateFromRequestAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr
        };

        await AddAsync(category);
        await SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            NameEn = category.NameEn,
            NameAr = category.NameAr
        };
    }
}

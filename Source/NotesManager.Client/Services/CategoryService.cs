using Microsoft.EntityFrameworkCore;
using NotesManager.Client.Data;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace NotesManager.Client.Services
{
    public class CategoryService : ICategoryService
    {
        protected readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Category>> AddCategoryAsync(Category category, CancellationToken token = default)
        {
            category.Trim();

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name, token);

            if (existingCategory is not null) return new ServiceResponse<Category>(false, "Category already exists");

            var newCategory = category.ToCategory();
            
            await _context.AddAsync(newCategory, token);
            await _context.SaveChangesAsync(token);

            return new ServiceResponse<Category>(true, "Category has been added", newCategory);
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(string categoryId, bool recursiveDelete = false, CancellationToken token = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { categoryId }, token);

            if (category is null) return new ServiceResponse(false, "Category not found");

            if (recursiveDelete && category.TextFiles.Any()) _context.TextFiles.RemoveRange(category.TextFiles);
            else if (!recursiveDelete && category.TextFiles.Any())
                return new ServiceResponse(false, "Category has related text files");

            _context.Remove(category);
            await _context.SaveChangesAsync(token);

            return new ServiceResponse(true, "Category deleted");
        }

        public async Task<ServiceResponse<Category>> EditCategoryAsync(Category category, CancellationToken token = default)
        {
            category.Trim();

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name && c.Id != category.Id, token);

            if (existingCategory is not null) return new ServiceResponse<Category>(false, "Category already exists");

            var oldCategory = await _context.Categories.FindAsync(new object[] { category.Id }, token);

            if (oldCategory is null) return new ServiceResponse<Category>(false, "Category does not exist");

            oldCategory.Name = category.Name;

            _context.Categories.Attach(oldCategory);

            await _context.SaveChangesAsync(token);

            return new ServiceResponse<Category>(true, "Category updated", oldCategory);
        }

        public async Task<ServiceResponse<ICollection<Category>>> GetAllCategoriesAsync(CancellationToken token = default)
            => new ServiceResponse<ICollection<Category>>(true, "Categories retrieved",
                data: await _context.Categories.ToListAsync(token));

        public async Task<ServiceResponse<int>> GetNumberOfRelatedFilesAsync(string categoryId, CancellationToken token = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { categoryId }, token);

            if (category is null) return new ServiceResponse<int>(false, "Category does not exist", 0);

            return new ServiceResponse<int>(true, string.Empty, category.TextFiles.Count);
        }

        public async Task<ServiceResponse<ICollection<TextFile>>> GetRelatedFilesAsync(string categoryId, CancellationToken token = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { categoryId }, token);

            if (category is null) return new ServiceResponse<ICollection<TextFile>>(false, "Cateogry does not exist");

            return new ServiceResponse<ICollection<TextFile>>(true, "Text files have been retrieved", category.TextFiles);
        }

        public async Task<ServiceResponse<ICollection<Category>>> SearchCategoriesByNameAsync(string text, CancellationToken token = default)
        {
            var categoriesQuery = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(text)) categoriesQuery = categoriesQuery.Where(c => c.Name.Contains(text.Trim()));

            var categories = await categoriesQuery.ToListAsync(token);

            var message = "No results found";

            if (categories.Count > 0)
                message = $"Found {categories.Count} result{(categories.Count > 1 ? "s" : "")}";

            return new ServiceResponse<ICollection<Category>>(true, message, categories);
        }
    }
}

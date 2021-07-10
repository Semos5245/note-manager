using NotesManager.Client.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotesManager.Client.Services
{
    public interface ICategoryService
    {
        Task<ServiceResponse<ICollection<Category>>> SearchCategoriesByNameAsync(string text, CancellationToken token = default);
        Task<ServiceResponse<ICollection<Category>>> GetAllCategoriesAsync(CancellationToken token = default);
        Task<ServiceResponse<Category>> AddCategoryAsync(Category category, CancellationToken token = default);
        Task<ServiceResponse<Category>> EditCategoryAsync(Category category, CancellationToken token = default);
        Task<ServiceResponse> DeleteCategoryAsync(string categoryId, bool recursiveDelete = false, CancellationToken token = default);
        Task<ServiceResponse<int>> GetNumberOfRelatedFilesAsync(string categoryId, CancellationToken token = default);
        Task<ServiceResponse<ICollection<TextFile>>> GetRelatedFilesAsync(string categoryId, CancellationToken token = default);
    }
}

using NotesManager.Client.Data.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotesManager.Client.Services
{
    public interface ITextFileService
    {
        Task<ServiceResponse<IEnumerable<TextFile>>> GetFilesForCategoryAsync(string categoryId, CancellationToken token = default);
        Task<ServiceResponse<TextFile>> GetTextFileAsync(string textFileId, CancellationToken token = default);
        Task<ServiceResponse<TextFile>> AddTextFileAsync(TextFile textFile, CancellationToken token = default);
        Task<ServiceResponse<TextFile>> SaveTextFileContentAsync(string textFileId, string newContent, CancellationToken token = default);
        Task<ServiceResponse<TextFile>> EditTextFileNameAsync(string textFileId, string newName, CancellationToken token = default);
        Task<ServiceResponse<IEnumerable<TextFileByContent>>> SearchTextFilesByContentAsync(string text, CancellationToken token = default);
        Task<ServiceResponse<bool>> ExistsWithinCategoryAsync(string fileName, string categoryId, CancellationToken token = default);
        Task<ServiceResponse<int>> ImportTextFilesAsync(IEnumerable<TextFileAdditionRequest> additionRequests, CancellationToken token = default);
        Task<ServiceResponse> DeleteTextFileAsync(string textFileId, CancellationToken token = default);
    }
}

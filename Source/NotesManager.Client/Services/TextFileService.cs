using Microsoft.EntityFrameworkCore;
using NotesManager.Client.Data;
using NotesManager.Client.Data.Models;
using NotesManager.Client.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;
using System.Text;

namespace NotesManager.Client.Services
{
    public class TextFileService : ITextFileService
    {
        protected readonly AppDbContext _context;

        public TextFileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<TextFile>> AddTextFileAsync(TextFile textFile, CancellationToken token = default)
        {
            textFile.Trim();

            if (string.IsNullOrEmpty(textFile.FileName))
                return new ServiceResponse<TextFile>(false, "File name can't be empty");

            var category = await _context.Categories
                .SingleOrDefaultAsync(c => c.Id == textFile.CategoryId, token);

            if (category is null) return new ServiceResponse<TextFile>(false, "Category does not exist");

            var existingFile = await _context.TextFiles
                .FirstOrDefaultAsync(t => t.FileName == textFile.FileName && t.CategoryId == category.Id, token);

            if (existingFile is not null) return new ServiceResponse<TextFile>(false, "File already exists");

            var newFile = textFile.ToTextFile();

            await _context.TextFiles.AddAsync(newFile, token);
            await _context.SaveChangesAsync(token);

            return new ServiceResponse<TextFile>(true, "File added", newFile);
        }

        public async Task<ServiceResponse<TextFile>> SaveTextFileContentAsync(string textFileId, string newContent, CancellationToken token = default)
        {
            var file = await _context.TextFiles.FindAsync(new object[] { textFileId }, token);

            if (file is null) return new ServiceResponse<TextFile>(false, "File does not exists");

            newContent = string.IsNullOrEmpty(newContent) ? string.Empty : newContent.Trim();

            file.Content = newContent;
            file.ModificationDateUtc = DateTime.UtcNow;

            _context.TextFiles.Attach(file);
            await _context.SaveChangesAsync(token);

            return new ServiceResponse<TextFile>(true, "Text file edited", file);
        }

        public async Task<ServiceResponse<bool>> ExistsWithinCategoryAsync(string fileName, string categoryId, CancellationToken token = default)
        {
            var category = await _context.Categories.FindAsync(new object[] { categoryId }, token);

            if (category is null)
                return new ServiceResponse<bool>(false, "Category does not exist", false);

            fileName = (string.IsNullOrEmpty(fileName) ? string.Empty : fileName.Trim());

            var exists = await _context.TextFiles.AnyAsync(t => t.FileName == fileName && t.CategoryId == categoryId, token);
            
            var files = await _context.TextFiles.Where(t => t.FileName == fileName).ToListAsync();

            return new ServiceResponse<bool>(true, string.Empty, data: exists);
        }

        public async Task<ServiceResponse<IEnumerable<TextFile>>> GetFilesForCategoryAsync(string categoryId, CancellationToken token = default)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(c => c.Id == categoryId, token);

            if (category is null) return new ServiceResponse<IEnumerable<TextFile>>(false, "Category does not exist");

            return new ServiceResponse<IEnumerable<TextFile>>(true, "Files retrieved", category.TextFiles);
        }

        public async Task<ServiceResponse<TextFile>> GetTextFileAsync(string textFileId, CancellationToken token = default)
        {
            var textFile = await _context.TextFiles.FindAsync(new object[] { textFileId }, token);

            if (textFile is null) return new ServiceResponse<TextFile>(false, "Text file does not exist");

            return new ServiceResponse<TextFile>(true, "File retrieved", textFile);
        }

        public async Task<ServiceResponse<int>> ImportTextFilesAsync(IEnumerable<TextFileAdditionRequest> additionRequests, CancellationToken token = default)
        {
            var erroredFiles = 0;

            foreach (var request in additionRequests)
            {
                var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, token);
                
                if (category == null)
                {
                    erroredFiles++;
                    continue;
                }

                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(request.FileName);

                var textFileByName = await _context.TextFiles
                    .SingleOrDefaultAsync(t => t.FileName == filenameWithoutExtension && t.CategoryId == category.Id, token);

                if (textFileByName is not null)
                {
                    switch (request.Action)
                    {
                        case RequestAction.Add:
                            throw new InvalidOperationException("Can't add file because it already exists");
                        case RequestAction.Append:
                            textFileByName.Content += request.Content;
                            _context.TextFiles.Attach(textFileByName);
                            break;
                        case RequestAction.Overwrite:
                            textFileByName.Content = request.Content;
                            _context.TextFiles.Attach(textFileByName);
                            break;
                    }

                    continue;
                }

                var textFile = new TextFile
                {
                    Content = request.Content,
                    FileName = filenameWithoutExtension,
                    CategoryId = category.Id
                };

                await _context.TextFiles.AddAsync(textFile, token);
            }

            if (erroredFiles == additionRequests.Count())
                return new ServiceResponse<int>(false, "There are errors in impored files", erroredFiles);

            await _context.SaveChangesAsync(token);

            var messageBuilder = new StringBuilder();
            messageBuilder.Append("Success");

            var numOfAddedFiles = additionRequests.Count() - erroredFiles;

            if (erroredFiles > 0)
                messageBuilder.Append($" with {erroredFiles} error{(erroredFiles > 1 ? "s" : "")}.");

            return new ServiceResponse<int>(true, messageBuilder.ToString(), erroredFiles);
        }

        public async Task<ServiceResponse<IEnumerable<TextFileByContent>>> SearchTextFilesByContentAsync(string text, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(text))
                return new ServiceResponse<IEnumerable<TextFileByContent>>(false, "search text can't be empty");
            
            var textFiles = await _context.TextFiles.Where(t => t.Content.Contains(text)).ToListAsync(token);

            var message = "No results found";

            if (textFiles.Count > 0)
                message = $"Found {textFiles.Count} result{(textFiles.Count > 1 ? "s" : "")}";

            return new ServiceResponse<IEnumerable<TextFileByContent>>(true, message,
                textFiles.Select(t => t.ToSearchTextFilesByContentResponse(text)));
        }

        public async Task<ServiceResponse<TextFile>> EditTextFileNameAsync(string textFileId, string newName, CancellationToken token = default)
        {
            var textFile = await _context.TextFiles.FindAsync(new object[] { textFileId }, token);

            if (textFile is null) return new ServiceResponse<TextFile>(false, "File does not exist");

            newName = newName.GetOrSetIfNull(string.Empty);

            var existingFile = await _context.TextFiles
                .FirstOrDefaultAsync(t => t.FileName == newName && t.Id != textFileId && t.CategoryId == textFile.CategoryId, token);

            if (existingFile is not null) return new ServiceResponse<TextFile>(false, "File already exists");

            textFile.FileName = newName;
            textFile.ModificationDateUtc = DateTime.UtcNow;

            _context.TextFiles.Attach(textFile);
            await _context.SaveChangesAsync(token);

            return new ServiceResponse<TextFile>(true, "File name has be edited", textFile);
        }

        public async Task<ServiceResponse> DeleteTextFileAsync(string textFileId, CancellationToken token = default)
        {
            var textFile = await _context.TextFiles.FindAsync(new object[] { textFileId }, token);

            if (textFile is null) return new ServiceResponse(false, "Text file does not exist");

            _context.TextFiles.Remove(textFile);
            await _context.SaveChangesAsync(token);

            return new ServiceResponse(true, "Text file deleted");
        }
    }
}

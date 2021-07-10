using NotesManager.Client.Data.Models;
using NotesManager.Client.Services;

namespace NotesManager.Client.Extensions
{
    public static class MappingExtensions
    {
        public static Category ToCategory(this Category category)
            => new Category
            {
                Name = category.Name
            };

        public static TextFile ToTextFile(this TextFile textFile)
            => new TextFile
            {
                FileName = textFile.FileName,
                CategoryId = textFile.CategoryId,
                Content = textFile.Content,
                CreationDateUtc = textFile.CreationDateUtc,
                ModificationDateUtc = textFile.ModificationDateUtc
            };

        public static TextFileByContent ToSearchTextFilesByContentResponse(this TextFile textFile, string text, int maxNumOfWordsOnLeft = 5, int maxNumOfWordsOnRight = 5)
        {
            var indexOfText = textFile.Content.IndexOf(text);

            (var displayText, var highlightStartIndex, var highlightEndIndex) = textFile.Content.TakeFromLeftAndRight(text, indexOfText, maxNumOfWordsOnLeft, maxNumOfWordsOnRight);

            return new TextFileByContent(textFile, displayText, indexOfText, indexOfText + text.Length, highlightStartIndex, highlightEndIndex);

        }

        public static (string, int, int) TakeFromLeftAndRight(this string content, string text, int startingIndex, int numOfWordsOnLeft = 5, int numOfWordsOnRight = 5)
        {
            var displayText = text;

            int numOfSpacesOnLeft = 0;
            int startIndex = 0;

            for (int i = startingIndex - 1; i >= 0; i--, startIndex = i)
            {
                var letter = content[i];

                if (letter == ' ') numOfSpacesOnLeft++;
                if (numOfSpacesOnLeft > numOfWordsOnLeft) break;

                displayText = $"{letter}{displayText}";
            }

            int numOfSpacesOnRight = 0;
            int endIndex = startingIndex + text.Length;

            for (int i = startingIndex + text.Length; i < content.Length; i++, endIndex = i)
            {
                var letter = content[i];

                if (letter == ' ') numOfSpacesOnRight++;
                if (numOfSpacesOnRight > numOfWordsOnLeft) break;

                displayText = $"{displayText}{letter}";
            }
            
            startIndex++;

            if (startIndex != 0) displayText = $".... {displayText}";
            if (endIndex != content.Length) displayText = $"{displayText} ....";

            return (displayText.Trim(), startIndex, endIndex);
        }

    }
}

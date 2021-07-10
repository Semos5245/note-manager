using NotesManager.Client.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotesManager.Client.Extensions
{
    public static class GeneralExtensions
    {
        public static int ToInt<T>(this T enumValue) where T : Enum => Convert.ToInt32(enumValue);
        public static string GetOrSetIfNull(this string value, string alternatingValue = "")
            => string.IsNullOrEmpty(value) ? alternatingValue : value.Trim();

        public static T Trim<T>(this T obj) where T: BaseEntity
        {
            var objType = typeof(T);

            foreach (var property in objType.GetProperties()
                .Where(p => p.CanWrite && p.CanWrite && p.PropertyType == typeof(string)))
            {
                var propertyValue = property.GetValue(obj);

                if (propertyValue != null && !string.IsNullOrEmpty((string)propertyValue))
                {
                    property.SetValue(obj, ((string)propertyValue).Trim());
                }
            }

            return obj;
        }

        public static bool HasDuplicates(this IEnumerable<string> collection)
        {
            foreach (var item in collection)
                if (collection.Count(i => i == item) > 1)
                    return true;
            return false;
        }
    }
}

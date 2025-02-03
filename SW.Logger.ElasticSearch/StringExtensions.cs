using System.Collections.Generic;
using System.Linq;

namespace SW.Logger.ElasticSerach
{
    public static class StringExtensions
    {
        public static (bool, string) IsValidIndexName(this string name)
        {
            var errorMessages = new List<string>();
            var invalidChars = new List<char> {'/', '\\', '*', '?', '"', '<', '>', '\'', '|',':'};
            if (name.Any(char.IsUpper))
                errorMessages.Add("Elastic search cannot contain upper case letters");
            foreach (var notAllowedChar in invalidChars)
            {
                if (name.Contains(notAllowedChar))
                    errorMessages.Add($"Name can't contain: '{notAllowedChar}' character(s)");
            }

            var invalidStartChars = new List<char> {'-', '_', '+'};

            foreach (var invalidStartChar in invalidStartChars)
            {
                if (name.StartsWith(invalidStartChar))
                    errorMessages.Add($"Name can't start with '{invalidStartChar}' character(s)");
            }

            if (!errorMessages.Any()) return (true, null);
            var message = string.Join(", ", errorMessages);
            return (false,
                $"Invalid ApplicationName '{name}' in SwLogger environment settings with validation errors: {message}");

        }
    }
}
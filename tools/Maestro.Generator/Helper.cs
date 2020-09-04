using System.Collections.Generic;

namespace Maestro.Generator
{
    public static class Helper
    {
        public static string ToC(this string input) => input.ToLowerInvariant().Replace(' ', '_');

        public static IEnumerable<string> IsC(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                yield return "cannot be empty";
            if (input.Contains(" "))
                yield return "cannot contain whitespace (replace by underscore or camel case)";
        }
    }
}

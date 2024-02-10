using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Todo.Blazor.Helpers
{
    public class StripHTML
    {
        public static string GetText(string htmlString)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }
        public static string GetText(string htmlString, int words = 0)
        {
            string pattern = @"<(.|\n)*?>";
            var text = Regex.Replace(htmlString, pattern, string.Empty);

            if (words == 0)
                return text;
            else
                return String.Join(" ", text.Split(" ").Take(words));
        }
    }
}

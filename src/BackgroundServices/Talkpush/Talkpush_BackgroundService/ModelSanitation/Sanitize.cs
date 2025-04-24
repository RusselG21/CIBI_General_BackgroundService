using System.Text.RegularExpressions;

namespace Talkpush_BackgroundService.ModelSanitation
{
    public class Sanitize
    {
        public static string StringSanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return Regex.Replace(input.Trim(), @"[^\u0020-\u007E]", ""); // removes non-ASCII
        }
    }
}

using System.Net;
using System.Text.RegularExpressions;

namespace BlogApp.Helper
{
  static  public class RemoveHtmlTagHelper
    {
        public static string RemoveHtmlTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove HTML tags
            string noHtml = Regex.Replace(input, "<[^>]+>", string.Empty);

            // Decode HTML entities (&nbsp; &amp; etc.)
            return WebUtility.HtmlDecode(noHtml);
        }
    }
}

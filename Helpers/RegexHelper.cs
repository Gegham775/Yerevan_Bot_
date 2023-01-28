using System.Text.RegularExpressions;

namespace Yerevan_Housing_API.Helpers
{
    public class RegexHelper
    {
        public static string GetImageId(string url)
        {
            var imageId = string.Empty;
            var regex = new Regex("\\=(.*)");
            Match match = regex.Match(url);
            if (match.Success) imageId = match.Groups[1].Value;
            return imageId;
        }
    }
}

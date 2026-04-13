using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AzureShoppingCart.Common;

public static class SlugHelper
{
    public static string GenerateSlug(this string phrase)
    {
        string str = phrase.RemoveDiacritics().ToLower();

        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        
        str = Regex.Replace(str, @"\s+", " ").Trim();
  
        str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        str = Regex.Replace(str, @"\s", "-");
        return str;
    }


    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}

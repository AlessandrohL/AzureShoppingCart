using System.Text;
using System.Text.RegularExpressions;

namespace AzureShoppingCart.Extensions;

public static class FileExtensions
{
    private static readonly string[] _permittedImageTypes = ["image/jpg", "image/jpeg", "image/pjpeg", "image/webp", "image/png"];
    private static readonly string[] _permittedImageExtensions = [".jpg", ".jpeg", ".pjpeg", ".png", ".webp"];
    private const int ImageMinimumBytes = 512;

    public static bool IsImage(this IFormFile formFile)
    {
        if (formFile is null)
            return false;

        if (!_permittedImageTypes.Contains(formFile.ContentType.ToLower()))
            return false;

        if (!_permittedImageExtensions.Contains(Path.GetExtension(formFile.FileName).ToLower()))
            return false;

        try
        {
            if (!formFile.OpenReadStream().CanRead || formFile.Length < ImageMinimumBytes)
                return false;

            byte[] buffer = new byte[ImageMinimumBytes];
            formFile.OpenReadStream().ReadExactly(buffer, 0, ImageMinimumBytes);
            string content = Encoding.UTF8.GetString(buffer);
            if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            formFile.OpenReadStream().Position = 0;
        }

        return true;
    }
}

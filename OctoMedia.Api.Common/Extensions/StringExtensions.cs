namespace OctoMedia.Api.Common.Extensions
{
    public static class StringExtensions
    {
        public static string? NullIfEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
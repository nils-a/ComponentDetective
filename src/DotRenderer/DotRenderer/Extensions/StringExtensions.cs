namespace ComponentDetective.DotRenderer.Extensions
{
    internal static class StringExtensions
    {
        internal static string AsDotIdentifier(this string s)
        {
            return $"\"{s.Replace("\"", "\\\"")}\"";
        }
    }
}

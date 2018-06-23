using Contracts.Models;

namespace ComponentDetective.DotRenderer.Extensions
{
    internal static class LibraryReferenceExtensions
    {
        internal static string GetId(this ILibraryReference info)
        {
            if (!string.IsNullOrEmpty(info.HintPath))
            {
                return info.HintPath.AsDotIdentifier();
            }
            return info.Name.AsDotIdentifier();

        }
    }
}

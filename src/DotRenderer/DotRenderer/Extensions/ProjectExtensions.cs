using Contracts.Models;

namespace ComponentDetective.DotRenderer.Extensions
{
    internal static class ProjectExtensions
    {
        internal static string GetId(this IProjectInformation info)
        {
            return info.Path.AsDotIdentifier();
        }
        internal static string GetId(this IProjectReference info)
        {
            return info.Path.AsDotIdentifier();
        }
    }
}

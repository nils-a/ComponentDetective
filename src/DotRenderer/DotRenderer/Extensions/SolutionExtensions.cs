using Contracts.Models;

namespace ComponentDetective.DotRenderer.Extensions
{
    internal static class SolutionExtensions
    {
        internal static string GetId(this ISolutionInformation info)
        {
            return info.Path.AsDotIdentifier();
        }
    }
}

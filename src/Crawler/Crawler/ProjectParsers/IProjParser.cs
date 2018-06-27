using ComponentDetective.Crawler.Models;

namespace ComponentDetective.Crawler.ProjectParsers
{
    internal interface IProjParser
    {
        ProjectInformation Parse(string projFilePath);
    }
}

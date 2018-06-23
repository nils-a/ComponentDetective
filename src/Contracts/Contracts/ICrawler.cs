using Contracts.Models;

namespace ComponentDetective.Contracts
{
    public interface ICrawler
    {
        IComponentOverview Crawl(string basePath);
    }
}


namespace ModernBlog.Services;

public interface IAiHandler
{
    Task<string> GetContent(string prompt);
    Task<string> GetIntroduction(string prompt);
}
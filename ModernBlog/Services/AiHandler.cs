using Mscc.GenerativeAI;

namespace ModernBlog.Services;

public class AiHandler(ILogger<AiHandler> logger, IConfiguration configuration) : IAiHandler
{
    private readonly IConfiguration configuration = configuration;
    private readonly ILogger<AiHandler> logger = logger;
    private ChatSession? contentChatSession;
    private GoogleAI? googleAI;
    private ChatSession? introductionChatSession;

    public async Task<string> GetContent(string prompt)
    {
        var chatSession = GetChatSession(BlogContent.Content);
        var response = await chatSession.SendMessage(prompt);
        return response == null ? "No response from AI" : response.Text ?? "No response from AI";
    }

    public async Task<string> GetIntroduction(string prompt)
    {
        var chatSession = GetChatSession(BlogContent.Introduction);
        var response = await chatSession.SendMessage(prompt);
        return response == null ? "No response from AI" : response.Text ?? "No response from AI";
    }

    private ChatSession GetChatSession(BlogContent contentType)
    {
        if (contentType == BlogContent.Introduction)
        {
            googleAI ??= new GoogleAI(apiKey: configuration["Gemini:ApiKey"], logger: logger);
            if (introductionChatSession == null)
            {
                var systemInstructionsShort = "You are a helpful assistant. " +
                    "You will respond answers in plain text only. " +
                    "Your answers would be around 200 words long.";
                GenerativeModel modelShort = googleAI.GenerativeModel(model: Model.Gemini15Flash, systemInstruction: new Content(systemInstructionsShort));
                introductionChatSession = modelShort.StartChat();
            }
            return introductionChatSession;
        }
        else
        {
            googleAI ??= new GoogleAI(apiKey: configuration["Gemini:ApiKey"], logger: logger);
            if (contentChatSession == null)
            {
                var systemInstructionsLong = "You are a helpful assistant. " +
                    "Your response would give detailed and explanations." +
                    "Your answers would be around 1500 to 2000 words long, depending on the topic." +
                    "You response would be in markdown format.";
                GenerativeModel modelLong = googleAI.GenerativeModel(model: Model.Gemini15Flash, systemInstruction: new Content(systemInstructionsLong));
                contentChatSession = modelLong.StartChat();
            }
            return contentChatSession;
        }
    }
}

public enum BlogContent
{
    Introduction,
    Content
}
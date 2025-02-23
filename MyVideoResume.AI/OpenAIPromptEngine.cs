using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntimeGenAI;
using MyVideoResume.Abstractions.Core;
using OpenAI.Chat;
using TiktokenSharp;

namespace MyVideoResume.AI;

public class OpenAIPromptEngine : IPromptEngine
{
    protected readonly ILogger<OpenAIPromptEngine> _logger;
    protected readonly IConfiguration _configuration;
    private ChatClient? client = null;
    private static TikToken _tokenizer;

    public OpenAIPromptEngine(ILogger<OpenAIPromptEngine> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _tokenizer = TikToken.EncodingForModel("gpt-4");
    }

    public async Task<ResponseResult> Process(string question)
    {
        var systemPrompt = "You are an AI assistant that helps people summarize work history and resume experience.";
        return await Process(systemPrompt, question);
    }

    public async Task<ResponseResult> Process(string prompt, string question)
    {
        return await Process(prompt, new[] { question });
    }

    public async Task<ResponseResult> Process(string prompt, string[] questions)
    {
        if (client == null)
        {
            var key = _configuration.GetValue<string>("AI:OpenAIKey");
            var model = _configuration.GetValue<string>("AI:OpenAIModel");
            client = new(model: model, apiKey: key);
        }

        var c = ChatMessage.CreateSystemMessage(prompt);

        List<ChatMessage> chatHistory = new() { c };

        foreach (var message in questions)
        {
            var truncated = TruncateToMaxTokens(message);
            chatHistory.Add(ChatMessage.CreateUserMessage(truncated));
        }

        var chatResult = await client.CompleteChatAsync(chatHistory);
        var result = new ResponseResult() { Result = chatResult.Value.Content[0].Text };
        return result;
    }

    public static string TruncateToMaxTokens(string input)
    {
        int maxTokens = 127500;

        var encoding = _tokenizer.Encode(input);

        if (encoding.Count <= maxTokens)
        {
            return input;  // Text is already within limit, return as is.
        }
        else
        {
            encoding = encoding.GetRange(0, maxTokens);
        }

        // Decode the truncated tokens back to text
        string truncatedText = _tokenizer.Decode(encoding);

        return truncatedText;
    }
}

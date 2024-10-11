using MessageSilo.Domain.Interfaces;
using OpenAI.Chat;

namespace MessageSilo.Infrastructure.Services
{
    public class AIService : IAIService
    {
        private readonly ChatClient client;

        public AIService(string apiKey, string model)
        {
            client = new(model: model ?? "gpt-4o", apiKey);
        }

        public async Task<string> Chat(string command, string message)
        {
            IEnumerable<ChatMessage> chatMessages =
            [
                ChatMessage.CreateSystemMessage(command),
                ChatMessage.CreateSystemMessage("Ensure the output is strictly formatted as valid JSON with no additional text or comments."),
                ChatMessage.CreateUserMessage(message)
            ];

            ChatCompletion completion = await client.CompleteChatAsync(chatMessages, options: new ChatCompletionOptions()
            {
                ResponseFormat = ChatResponseFormat.JsonObject
            });

            return completion.ToString();
        }
    }
}

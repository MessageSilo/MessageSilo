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

        public async Task<string> Chat(string prompt, string message)
        {
            IEnumerable<ChatMessage> chatMessages =
            [
                ChatMessage.CreateSystemMessage(prompt),
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

using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using RestSharp;
using System.Text.RegularExpressions;

namespace MessageSilo.Features.Enricher
{
    public class AIEnricher : IEnricher
    {
        private readonly string apiKey;

        private readonly string command;

        private OpenAIService service;

        public AIEnricher(string apiKey, string command)
        {
            this.apiKey = apiKey;
            this.command = command;

            service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = apiKey
            });
        }

        public async Task<string> TransformMessage(string message)
        {
            var completionResult = await service.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(command),
                    ChatMessage.FromSystem("You must respond only the transofrmed JSON, and nothing else!"),
                    ChatMessage.FromUser(message),
                },
                Model = Models.ChatGpt3_5Turbo,
            });

            if (!completionResult.Successful)
                throw new Exception(completionResult?.Error?.Message);

            return completionResult.Choices.First().Message.Content;
        }
    }
}

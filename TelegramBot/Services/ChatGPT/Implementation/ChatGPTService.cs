using System.Net.Http.Json;
using TelegramBot.Services.ChatGPT.Interface;
using TelegramBot.Services.ChatGPT.Models;

namespace TelegramBot.Services.ChatGPT.Implementation
{
    public class ChatGPTService : IChatGPTService
    {
        private string _apiKey = ""; // Write your api key
        private string _endpoint = "https://api.openai.com/v1/chat/completions";
        private string _responseText = string.Empty;

        public async Task<string> Chat(string content)
        {
            List<Message> messages = new List<Message>();

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            while (true)
            {
                if (content is not { Length: > 0 }) break;
                var message = new Message() { Role = "user", Content = content };
                messages.Add(message);

                var requestData = new Request()
                {
                    ModelId = "gpt-3.5-turbo",
                    Messages = messages
                };
                using var response = await httpClient.PostAsJsonAsync(_endpoint, requestData);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"{(int)response.StatusCode} {response.StatusCode}");
                    break;
                }
                ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

                var choices = responseData?.Choices ?? new List<Choice>();
                if (choices.Count == 0)
                {
                    Console.WriteLine("No choices were returned by the API");
                    continue;
                }
                var choice = choices[0];
                var responseMessage = choice.Message;
                messages.Add(responseMessage);
                _responseText = responseMessage.Content.Trim();
            }
            return _responseText;
        }
    }
}

namespace TelegramBot.Services.ChatGPT.Interface
{
    public interface IChatGPTService
    {
        Task<string> Chat(string content);
    }
}

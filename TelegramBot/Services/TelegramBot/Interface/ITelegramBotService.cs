namespace TelegramBot.Services.TelegramBot.Interface
{
    public interface ITelegramBotService
    {
        Task InitializeBotAsync();
        Task InitializeBotWithChatGPTAsync();
    }
}

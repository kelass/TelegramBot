using Telegram.Bot;

namespace TelegramBot.Services.FileSaver.Interface
{
    public interface IFileSaverService
    {
        Task<string> SaveFile(Telegram.Bot.Types.File file, ITelegramBotClient _botClient);
    }
}

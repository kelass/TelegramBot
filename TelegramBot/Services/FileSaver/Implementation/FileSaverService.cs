using Telegram.Bot;
using TelegramBot.Services.FileSaver.Interface;

namespace TelegramBot.Services.FileSaver.Implementation
{
    public class FileSaverService : IFileSaverService
    {
        private readonly string _currentDirectory;
        public FileSaverService()
        {
            _currentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "files");
        }

        public async Task<string> SaveFile(Telegram.Bot.Types.File file, ITelegramBotClient _botClient)
        {
            using (var fileStream = new MemoryStream())
            {
                if (!Directory.Exists(_currentDirectory))
                {
                    Directory.CreateDirectory(_currentDirectory);
                }

                string fileName = $"{Guid.NewGuid().ToString()}.png";
                await _botClient.DownloadFileAsync(file.FilePath, fileStream);
                byte[] bytes = fileStream.ToArray();
                await System.IO.File.WriteAllBytesAsync(Path.Combine(_currentDirectory, fileName), bytes);

                Console.WriteLine($"File saved as: {fileName}");

                return fileName;
            }
        }
    }
}

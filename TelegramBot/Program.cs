using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBot.Services.ChatGPT.Implementation;
using TelegramBot.Services.ChatGPT.Interface;
using TelegramBot.Services.FileSaver.Implementation;
using TelegramBot.Services.FileSaver.Interface;
using TelegramBot.Services.Mindee.Implementation;
using TelegramBot.Services.Mindee.Interface;
using TelegramBot.Services.TelegramBot.Implementation;
using TelegramBot.Services.TelegramBot.Interface;

namespace TelegramBot
{
    public class Program
    {
        static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddScoped<IMindeeService, MindeeService>()
                        .AddScoped<ITelegramBotService, TelegramBotService>()
                        .AddScoped<IChatGPTService, ChatGPTService>()
                        .AddScoped<IFileSaverService, FileSaverService>());

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var _telegramBotService = host.Services.GetRequiredService<ITelegramBotService>();
            await _telegramBotService.InitializeBotWithChatGPTAsync();
        }
    }
}

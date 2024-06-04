using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services.ChatGPT.Interface;
using TelegramBot.Services.FileSaver.Interface;
using TelegramBot.Services.Mindee.Interface;
using TelegramBot.Services.TelegramBot.Interface;

namespace TelegramBot.Services.TelegramBot.Implementation
{
    public class TelegramBotService : ITelegramBotService
    {
        private string _botToken = "7159002536:AAF-g3o-PRb5RgMT8bvQCYa_n3WZdF9Jm2w";
        private ITelegramBotClient _botClient;
        private string Name = string.Empty;
        private Dictionary<long, List<Telegram.Bot.Types.File>> userDocuments = new();
        private readonly IMindeeService _mindeeService;
        private readonly IChatGPTService _chatGptService;
        private readonly IFileSaverService _fileSaverService;
        private string passportData = string.Empty;
        private string licenseData = string.Empty;

        public TelegramBotService(IMindeeService mindeeService, IChatGPTService chatGptService, IFileSaverService fileSaverService)
        {
            _mindeeService = mindeeService;
            _chatGptService = chatGptService;
            _fileSaverService = fileSaverService;
        }

        public async Task InitializeBotAsync()
        {
            _botClient = new TelegramBotClient(_botToken);

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        public async Task InitializeBotWithChatGPTAsync()
        {
            _botClient = new TelegramBotClient(_botToken);

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                HandleChatGPTUpdateAsync,
                HandleChatGPTErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        #region private
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message) return;
            var message = update.Message;

            if (message?.Type == MessageType.Text)
            {
                await MessageText(message);
            }
            else if (message?.Type == MessageType.Photo)
            {
                await MessagePhoto(message);
            }
        }

        private Task HandleChatGPTErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }

        private async Task HandleChatGPTUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message) return;
            var message = update.Message;

            if (message?.Type == MessageType.Text)
            {
                await MessageGptText(message);
            }
            else if (message?.Type == MessageType.Photo)
            {
                await MessageGptPhoto(message);
            }
        }

        private async Task MessageText(Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello, I will help you to buy car insurance. Please send a photo of your passport (png, jpg, jpeg)");
            }
            else if (userDocuments.ContainsKey(message.Chat.Id) && userDocuments[message.Chat.Id].Count == 2)
            {
                if (message.Text.ToLower() == "/accept")
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Data confirmed. The cost of the insurance is 100 USD. Do you agree?\n Click /yes if you agree\n Click /no if dont agree");
                }
                else if (message.Text.ToLower() == "/yes")
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, $"The policy has been successfully established:\n{passportData + licenseData}");
                    userDocuments.Remove(message.Chat.Id);
                }
                else if (message.Text.ToLower() == "/no")
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Sorry this is the only price available at the moment\n Press /yes if you agree to continue or /exit if you want to finish ");
                }
                else if (message.Text.ToLower() == "/exit")
                {
                    userDocuments.Remove(message.Chat.Id);
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Thank you for using our bot");
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(message.Chat.Id, "I don't understand this command. Please start with /start.");
            }
        }

        private async Task MessagePhoto(Message message)
        {
            if (!userDocuments.ContainsKey(message.Chat.Id))
            {
                userDocuments[message.Chat.Id] = new List<Telegram.Bot.Types.File>();
            }

            var file = await _botClient.GetFileAsync(message.Photo.Last().FileId);
            userDocuments[message.Chat.Id].Add(file);

            if (userDocuments[message.Chat.Id].Count == 1)
            {
                var filePath = await _fileSaverService.SaveFile(file, _botClient);
                passportData = await _mindeeService.PostPasportMindeeAPI(Path.Combine("files", filePath));
                await _botClient.SendTextMessageAsync(message.Chat.Id, "Passport photo received. Please send a photo of the vehicle identification document.");
            }
            else if (userDocuments[message.Chat.Id].Count == 2)
            {
                var filePath = await _fileSaverService.SaveFile(file, _botClient);
                licenseData = await _mindeeService.PostLicenceMindeeAPI(Path.Combine("files", filePath));
                await _botClient.SendTextMessageAsync(message.Chat.Id, $"{passportData + licenseData}\nPlease confirm the data (click to /accept or send a new photo).");
            }
        }

        private async Task MessageGptText(Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                var text = await _chatGptService.Chat("generate text of a similar nature with more water: 'Hello, I will help you to buy car insurance. Please send a photo of your passport (png, jpg, jpeg)'");
                await _botClient.SendTextMessageAsync(message.Chat.Id, text);
            }
            else if (userDocuments.ContainsKey(message.Chat.Id) && userDocuments[message.Chat.Id].Count == 2)
            {
                if (message.Text.ToLower() == "/accept")
                {
                    var text = await _chatGptService.Chat("generate text of a similar nature with more water :'Data confirmed. The cost of the insurance is 100 USD. Do you agree?\n Click /yes if you agree\n Click /no if dont agree'");
                    await _botClient.SendTextMessageAsync(message.Chat.Id, text);
                }
                else if (message.Text.ToLower() == "/yes")
                {
                    var text = await _chatGptService.Chat($"add more text to the given text without changing the sensitive data create policy data for five years from today.:'The policy has been successfully established:\n{passportData + licenseData}'");
                    await _botClient.SendTextMessageAsync(message.Chat.Id, text);
                    userDocuments.Remove(message.Chat.Id);
                }
                else if (message.Text.ToLower() == "/no")
                {
                    var text = await _chatGptService.Chat("generate text of a similar nature with more water :'Sorry this is the only price available at the moment\n Press /yes if you agree to continue or /exit if you want to finish'");

                    await _botClient.SendTextMessageAsync(message.Chat.Id, text);
                }
                else if (message.Text.ToLower() == "/exit")
                {
                    var text = await _chatGptService.Chat("generate text of a similar nature with more water :'Thank you for using our bot'");
                    userDocuments.Remove(message.Chat.Id);
                    await _botClient.SendTextMessageAsync(message.Chat.Id, text);
                }
                else
                {
                    userDocuments.Remove(message.Chat.Id);
                    var text = await _chatGptService.Chat("generate text of a similar nature with more water :'Please send a photo of your passport.'");
                    await _botClient.SendTextMessageAsync(message.Chat.Id, text);
                }
            }
            else
            {
                var text = await _chatGptService.Chat("generate text of a similar nature with more water :'I don't understand this command. Please start with /start.'");
                await _botClient.SendTextMessageAsync(message.Chat.Id, text);
            }
        }

        private async Task MessageGptPhoto(Message message)
        {
            if (!userDocuments.ContainsKey(message.Chat.Id))
            {
                userDocuments[message.Chat.Id] = new List<Telegram.Bot.Types.File>();
            }

            var file = await _botClient.GetFileAsync(message.Photo.Last().FileId);
            userDocuments[message.Chat.Id].Add(file);

            if (userDocuments[message.Chat.Id].Count == 1)
            {
                var filePath = await _fileSaverService.SaveFile(file, _botClient);
                passportData = await _mindeeService.PostPasportMindeeAPI(Path.Combine("files", filePath));
                var text = await _chatGptService.Chat("generate text of a similar nature with more water :'Passport photo received. Please send a photo of the vehicle identification document.'");
                await _botClient.SendTextMessageAsync(message.Chat.Id, text);
            }
            else if (userDocuments[message.Chat.Id].Count == 2)
            {
                var filePath = await _fileSaverService.SaveFile(file, _botClient);
                licenseData = await _mindeeService.PostLicenceMindeeAPI(Path.Combine("files", filePath));
                var text = await _chatGptService.Chat($"add more text to the given text without changing the sensitive data and dont remember write /accept :'Extracted Data:\n{passportData + licenseData}\nPlease confirm the data (click to /accept or send a new photo).'");
                await _botClient.SendTextMessageAsync(message.Chat.Id, text);
            }
        }
        #endregion
    }
}

namespace TelegramBot.Services.Mindee.Interface
{
    public interface IMindeeService
    {
        string SimulateMindeeAPI();
        Task<string> PostLicenceMindeeAPI(string file);
        Task<string> PostPasportMindeeAPI(string file);
    }
}

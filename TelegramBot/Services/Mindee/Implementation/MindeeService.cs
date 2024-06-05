using Mindee;
using Mindee.Input;
using Mindee.Product.InternationalId;
using Mindee.Product.Us.DriverLicense;
using TelegramBot.Services.Mindee.Interface;

namespace TelegramBot.Services.Mindee.Implementation
{
    public class MindeeService : IMindeeService
    {
        string apiKey = "4e221ec18d47aa92ff63e44a8e5eebf6";

        public string SimulateMindeeAPI()
        {
            return "Name/Last name: Ivanov Ivan\nDate of birth: 01.01.1990\nVehicle number: А123БВ45";
        }

        public async Task<string> PostLicenceMindeeAPI(string file)
        {
            try
            {
                MindeeClient mindeeClient = new MindeeClient(apiKey);

                var inputSource = new LocalInputSource(file);

                var response = await mindeeClient
                    .ParseAsync<DriverLicenseV1>(inputSource);

                return $"First/Last names: {response.Document.Inference.Prediction.FirstName}/ {response.Document.Inference.Prediction.LastName}\n " +
                    $"Address: {response.Document.Inference.Prediction.Address}" +
                    $"Driver licencse ID: {response.Document.Inference.Prediction.DriverLicenseId}\n";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public async Task<string> PostPasportMindeeAPI(string file)
        {
            try
            {
                MindeeClient mindeeClient = new MindeeClient(apiKey);

                var inputSource = new LocalInputSource(file);

                var response = await mindeeClient
                .EnqueueAndParseAsync<InternationalIdV2>(inputSource);

                return $"Extracted data:\n" +
                    $" Document number: {response.Document.Inference.Prediction.DocumentNumber}\n" +
                    $" Birth date: {response.Document.Inference.Prediction.BirthDate}\n" +
                    $" Exp date: {response.Document.Inference.Prediction.ExpiryDate}\n";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}

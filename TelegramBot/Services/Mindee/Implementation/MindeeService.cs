using Mindee;
using Mindee.Input;
using Mindee.Product.InternationalId;
using Mindee.Product.Us.DriverLicense;
using TelegramBot.Services.Mindee.Interface;

namespace TelegramBot.Services.Mindee.Implementation
{
    public class MindeeService : IMindeeService
    {
        string apiKey = "0b73ba4b105d4adc6855a07aad3356f7";

        public string SimulateMindeeAPI()
        {
            return "Name/Last name: Ivanov Ivan\nDate of birth: 01.01.1990\nVehicle number: А123БВ45";
        }

        public async Task<string> PostLicenceMindeeAPI(string file)
        {
            MindeeClient mindeeClient = new MindeeClient(apiKey);

            var inputSource = new LocalInputSource(file);

            var response = await mindeeClient
                .ParseAsync<DriverLicenseV1>(inputSource);

            return $"First/Last names: {response.Document.Inference.Prediction.FirstName}/ {response.Document.Inference.Prediction.LastName}\n " +
                $"Address: {response.Document.Inference.Prediction.Address}" +
                $"Driver licencse ID: {response.Document.Inference.Prediction.DriverLicenseId}\n";
        }

        public async Task<string> PostPasportMindeeAPI(string file)
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
    }
}

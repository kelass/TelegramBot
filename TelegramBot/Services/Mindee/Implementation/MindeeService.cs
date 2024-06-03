using Mindee;
using Mindee.Input;
using Mindee.Product.Us.DriverLicense;
using TelegramBot.Services.Mindee.Interface;

namespace TelegramBot.Services.Mindee.Implementation
{
    public class MindeeService : IMindeeService
    {
        string apiKey = "my-api-key-here";
        public string SimulateMindeeAPI()
        {
            return "Name/Last name: Ivanov Ivan\nDate of birth: 01.01.1990\nVehicle number: А123БВ45";
        }

        //public string PostMindeeAPI()
        //{
        //    MindeeClient mindeeClient = new MindeeClient(apiKey);

        //    var inputSource = new LocalInputSource(filePath);

        //    var response = await mindeeClient
        //        .ParseAsync<DriverLicenseV1>(inputSource);

        //    System.Console.WriteLine(response.Document.ToString());
        //}
    }
}

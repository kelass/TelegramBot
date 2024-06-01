using TelegramBot.Services.Mindee.Interface;

namespace TelegramBot.Services.Mindee.Implementation
{
    public class MindeeService:IMindeeService
    {
        public string SimulateMindeeAPI()
        {
            return "Name/Last name: Ivanov Ivan\nDate of birth: 01.01.1990\nVehicle number: А123БВ45";
        }
    }
}

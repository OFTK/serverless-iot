using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice
{
    class Program
    {
        private const int TakeSpot = 1;
        private const int LeaveSpot = 2;

        private const string DeviceConnectionString = @"";

        private static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);

        static async Task Main(string[] args)
        {
            while (true)
            {
                int action = GetSimulatedAction();

                var data = new { Action = action, ActionType = "SpotStateChange" };
                string messageJson = JsonConvert.SerializeObject(data);
                Message message = new Message(Encoding.ASCII.GetBytes(messageJson));

                await Client.SendEventAsync(message);
            }
        }

        private static int GetSimulatedAction()
        {
            while (true)
            { 
                Console.Clear();
                Console.WriteLine("Please select action number: ");
                Console.WriteLine($"({TakeSpot}) Take Spot");
                Console.WriteLine($"({LeaveSpot}) Leave Spot");

                char keyChar = Console.ReadKey(true).KeyChar;
                if (char.IsNumber(keyChar))
                {
                    int number = (int)char.GetNumericValue(keyChar);
                    if (number == TakeSpot || number == LeaveSpot)
                        return number;
                }
            }
        }

        
    }
}
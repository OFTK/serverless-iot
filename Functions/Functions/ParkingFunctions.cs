using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions
{
    public static class ParkingFunctions
    {
        private const string ParkingStateHubName = "ParkingState";

        [FunctionName("react-spot-state-changes")]
        public static async Task Run(
            [EventHubTrigger("spot-state-changes", Connection = "EventHubConnectionString")]
            string messageBody,
            [SignalR(HubName = ParkingStateHubName)] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");

            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "parking-state-change",
                Arguments = new object[] { JsonConvert.DeserializeObject(messageBody) }
            });
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo NegotiateConnection(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request,
            [SignalRConnectionInfo(HubName = ParkingStateHubName)]SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
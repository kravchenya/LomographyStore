using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using LomographyStoreApi.Services.Interfaces;
using LomographyStoreApi.Models;

namespace LomographyStoreApi.Services
{
    public class AzureQueueService : IQueueService
    {
        private readonly IConfiguration _config;
        private readonly QueueClient _orderQueueClient;

        public AzureQueueService(IConfiguration configuration)
        {
            _config = configuration;

            var connectionStr = _config[Constants.KEY_STORAGE_CNN];
            var queueName = _config[Constants.KEY_QUEUE];
            _orderQueueClient = new QueueClient(connectionStr, queueName);
        }

        public async Task SendMessageAsync(Order item)
        {
            string msgBody = JsonConvert.SerializeObject(item);

            //Required if Azure Resource Manager was used to create infrastucture
            await _orderQueueClient.CreateIfNotExistsAsync();

            await _orderQueueClient.SendMessageAsync(msgBody);
        }
    }
}
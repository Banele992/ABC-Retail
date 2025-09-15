
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ABCRetail.Models;
using System.Text.Json;

namespace ABCRetail.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;

        public QueueStorageService(string storageConnectionString, string queueName) 
        {
            var queueServiceClient = new QueueServiceClient(storageConnectionString);
            _queueClient = queueServiceClient.GetQueueClient(queueName);
            _queueClient.CreateIfNotExists();
        }

        //Send Message to queue
        public async Task SendMessagesAsync(object message) 
        {
           //Convert message to object JSON string
            var messageJson = JsonSerializer.Serialize(message);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(messageJson)));
        }

        //Get Messages from queue for the log
        public async Task<List<QueueLogViewModel>> GetMessagesAsync()
        {
            var messageList = new List<QueueLogViewModel>();
            var messages = await _queueClient.PeekMessagesAsync(maxMessages: 32);

            foreach (PeekedMessage message in messages.Value)
            {
                messageList.Add(new QueueLogViewModel
                {
                    MessageId = message.MessageId,
                    InsertionTime = message.InsertedOn,
                    MessageText = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(message.Body.ToString()))
                });
            }

            return messageList;
        }


    }
}

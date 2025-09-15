using ABCRetail.Models;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public class OrderService
    {
        private readonly TableClient _tableClient;

        public OrderService(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        // Get order by rowKey
        public async Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<Order>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        // Add order
        public async Task AddOrderAsync(Order order)
        {
            order.PartitionKey = order.OrderName;
            order.RowKey = Guid.NewGuid().ToString();
            await _tableClient.AddEntityAsync(order);
        }

        // Update order
        public async Task UpdateOrderAsync(Order order)
        {
            await _tableClient.UpdateEntityAsync(order, ETag.All, TableUpdateMode.Replace);
        }

        // Delete order
        public async Task DeleteOrderAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}

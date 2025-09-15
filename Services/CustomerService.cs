using ABCRetail.Models;
using Azure.Data.Tables;
using Azure;

namespace ABCRetail.Services
{
    public class CustomerService
    {
        private readonly TableClient _tableClient;

        public CustomerService(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            _tableClient = serviceClient.GetTableClient("Customer");
            _tableClient.CreateIfNotExists();
        }

        // Customer Methods

        // Get all customers
        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();
            await foreach (var customer in _tableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        // Get customer by rowKey
        public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        // Add customer
        public async Task AddCustomerAsync(Customer customer)
        {
            customer.PartitionKey = customer.Address;
            customer.RowKey = Guid.NewGuid().ToString();

            await _tableClient.AddEntityAsync(customer);
        }

        // Update customer
        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
        }

        // Delete customer
        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }


}

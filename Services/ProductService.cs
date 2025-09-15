using ABCRetail.Models;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Services
{
    public class ProductService
    {
        private readonly TableClient _tableClient;

        public ProductService(string storageConnectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            _tableClient = serviceClient.GetTableClient("Product");
            _tableClient.CreateIfNotExists();
        }
        // Product Methods

        // Get product by rowKey
        public async Task<Product?> GetProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<Product>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }


        // Add product
        public async Task AddProductAsync(Product product)
        {
            product.PartitionKey = product.ProductCategory;
            product.RowKey = Guid.NewGuid().ToString();

            await _tableClient.AddEntityAsync(product);
        }

        // Update product
        public async Task UpdateProductAsync(Product product)
        {
            await _tableClient.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
        }

        // Delete product
        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        // Get all products by rowKey
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

    }
}

using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Customer : ITableEntity
    {
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? CustPhoto { get; set; }     
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}

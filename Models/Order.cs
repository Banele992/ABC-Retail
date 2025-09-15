using Azure.Data.Tables;
using Azure;

namespace ABCRetail.Models
{
    public class Order : ITableEntity
    {        
            public string? PartitionKey { get; set; }
            public string? RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
            public string? OrderName { get; set; }      
            public object Product { get; internal set; }
           public object Quantity { get; internal set; }
           public DateTime OrderDate { get; internal set; }
           public double Total { get; internal set; }
    }
 }



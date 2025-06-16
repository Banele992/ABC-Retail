namespace ABCRetailPOE.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? OrderName { get; set; }
        public DateTime OrderDate { get; internal set; }
        public double Total { get; internal set; }
    }
}

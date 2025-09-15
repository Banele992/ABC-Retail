namespace ABCRetail.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }        
        public decimal Total { get; set; }
    }
}

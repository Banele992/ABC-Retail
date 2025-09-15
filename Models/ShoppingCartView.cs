namespace ABCRetail.Models
{
    public class ShoppingCartView
    {
        public List<ShoppingCartItem> CartItems { get; set; }

        public decimal? TotalPrice { get; set; }

        public int? Totaltotal { get; set; }
    }
}

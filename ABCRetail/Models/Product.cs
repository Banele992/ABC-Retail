namespace ABCRetailPOE.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
    }
}

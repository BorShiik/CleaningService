namespace CleanDeal.Model.Models
{
    public class ProductOrderItem
    {
        public int Id { get; set; }
        public int ProductOrderId { get; set; }
        public ProductOrder ProductOrder { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
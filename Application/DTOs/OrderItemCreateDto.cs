
namespace Application.DTOs
{
    public class OrderItemCreateDto
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

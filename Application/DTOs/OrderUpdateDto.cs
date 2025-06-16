using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }  // This is required for updates
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemUpdateDto> Items { get; set; } = new();
    }
}

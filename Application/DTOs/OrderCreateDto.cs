using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class OrderCreateDto
    {
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}

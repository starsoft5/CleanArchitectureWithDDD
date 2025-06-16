using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string OrderDate { get; set; } // formatted as string
        public List<OrderItemReadDto> Items { get; set; }
    }
}

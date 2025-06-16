using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime OrderDate { get; set; }
        [MaxLength(100)]
        public string Address { get; set; } = ""; 
        public List<OrderItem> Items { get; set; } = new List<OrderItem>(); // Fix IDE0028 by explicitly initializing  
    }
}

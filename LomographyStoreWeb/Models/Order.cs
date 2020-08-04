using System.Collections.Generic;

namespace LomographyStoreWeb.Models
{
    public class Order
    {
         public List<OrderItem> Items { get; set; }
    }
}
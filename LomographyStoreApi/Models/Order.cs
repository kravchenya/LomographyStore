using System.Collections.Generic;

namespace LomographyStoreApi.Models
{
    public class Order
    {
          public List<OrderItem> Items { get; set; }
    }
}
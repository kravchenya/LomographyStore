using Microsoft.Azure.Cosmos.Table;

namespace LomographyStoreApi.Models
{
     public class OrderHistoryItem : TableEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Camera { get; set; }
        public string Description { get; set; }
    }
}
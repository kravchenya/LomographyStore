namespace LomographyStoreApi.Services
{
   public class CosmosDBServiceOptions
    {
        public string DBUri { get; set; }
        public string DBKey { get; set; }
        public string DBName { get; set; }
        public string DBCollection { get; set; }
        public string DBPartitionKey { get; set; }
    }
}
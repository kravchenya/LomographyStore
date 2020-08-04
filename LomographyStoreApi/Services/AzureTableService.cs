using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using LomographyStoreApi.Models;
using LomographyStoreApi.Services.Interfaces;

namespace LomographyStoreApi.Services
{
    public class AzureTableService : ITableService
    {
        private IConfiguration _config;
        private string _tableName;
        private string _partitionName;
        private  CloudStorageAccount _acct;

        public AzureTableService(IConfiguration config)
        {
            _config = config;
            _tableName = config[Constants.KEY_TABLE];
            _partitionName = config[Constants.KEY_TABLE_PARTITION]; 
            _acct = CloudStorageAccount.Parse(_config[Constants.KEY_STORAGE_CNN]);  
        }

        public async Task<List<OrderHistoryItem>> GetOrderHistoryAsync()
        {
           
            var tableClient = _acct.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_tableName);

            //Required if Azure Resource Manager was used to create infrastucture
            await table.CreateIfNotExistsAsync();

            var historyQuery = new TableQuery<OrderHistoryItem>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, _partitionName));

            TableContinuationToken queryToken = null;
            var tableItems = await table.ExecuteQuerySegmentedAsync<OrderHistoryItem>(historyQuery, queryToken);

            return tableItems.ToList();
        }
    }
}
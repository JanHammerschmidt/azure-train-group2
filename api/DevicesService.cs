
using Microsoft.Azure.Storage;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CosmosDB.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api
{
    public class DevicesService
    {
        CloudStorageAccount Account;
        CloudTableClient TableClient;
        public DevicesService(IConfiguration config)
        {
            Account = CloudStorageAccount.Parse(config["storage:connectionString"]);
            TableClient = Account.CreateCloudTableClient();
        }

        // public List<string> GetDevices()
        // {

        //     var sensorvalid = TableClient.GetTableReference("sensorvalid");
        //     var query = sensorvalid.CreateQuery<TableEntity>();
        //     // var tbl = tableServiceContext.CreateQuery<PartitionEntry>("TablePartitions");
        //     // return tbl.Where(i => i.PartitionKey == "<table name>").Select(i => new { PartitionKey = i.RowKey, });
        // }

        public async Task<List<SensorValue>> GetAllValidData()
        {
            var sensorvalid = TableClient.GetTableReference("sensorvalid");
            var query = sensorvalid.CreateQuery<SensorValue>();
            var nextQuery = query;
            var continuationToken = default(TableContinuationToken);
            var results = new List<SensorValue>();
            do
            {
                var queryResult = await query.ExecuteSegmentedAsync(continuationToken).ConfigureAwait(false);
                results.AddRange(queryResult.Results);
                continuationToken = queryResult.ContinuationToken;
                if (continuationToken != null && query.TakeCount.HasValue)
                {
                    //Query has a take count, calculate the remaining number of items to load.
                    var itemsToLoad = query.TakeCount.Value - results.Count;

                    //If more items to load, update query take count, or else set next query to null.
                    nextQuery = itemsToLoad > 0
                        ? query.Take(itemsToLoad)
                        : null;
                }
            }
            while (continuationToken != null && nextQuery != null);

            return results;
        }

        public List<int> GetDeviceHistory(string id)
        {

            return new List<int> { 1, 2, 3 };
        }
    }
}

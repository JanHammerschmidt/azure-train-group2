
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
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
            var query = new TableQuery<SensorValue>();
            var nextQuery = query;
            var continuationToken = default(TableContinuationToken);
            var results = new List<SensorValue>();
            do
            {
                var queryResult = await sensorvalid.ExecuteQuerySegmentedAsync(query, continuationToken).ConfigureAwait(false);
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

        async public Task<List<string>> GetDevices()
        {
            return (await GetAllValidData()).Select(x => x.deviceId).Distinct().ToList();
        }

        async public Task<List<SensorValue>> GetDeviceHistory(string id)
        {
            return (await GetAllValidData()).Where(x => x.deviceId == id).ToList();
        }
    }
}

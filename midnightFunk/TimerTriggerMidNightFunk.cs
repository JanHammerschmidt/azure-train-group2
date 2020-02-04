using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace CommaSoft.Gruppe2
{
    public static class TimerTriggerMidNightFunk
    {
        [FunctionName("TimerTriggerMidNightFunk")]
        public static async Task Run([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=rg2group2storage;AccountKey=DJIQH2Yy+aY8ywHhrp7Mn4dwXgVZ2k2FOG2CAkwVZC7f39qyouEqqeBXzgrvVNNY8hh3rVYmrmGV6ScpQ8NRxg==;EndpointSuffix=core.windows.net");
            var client = account.CreateCloudTableClient();
            var validCounters = await CountPackets(client, "valid");
            var invalidCounters = await CountPackets(client, "invalid");

            validCounters.Merge(invalidCounters);
            var totalPackets = validCounters.Sum(x => x.Value);
            log.LogInformation($"received {totalPackets} packets from {validCounters.Keys.Count} different devices.");

            var totalInvalidPackets = invalidCounters.Sum(x => x.Value);
            log.LogInformation($"received {totalInvalidPackets} invalid packets from {invalidCounters.Keys.Count} different devices.");

            var worstTenDevices = invalidCounters.OrderByDescending(pair => pair.Value).Take(10).Select(pair => pair.Key);

            var sb = new StringBuilder();
            sb.Append("Worst 10 devices:\n");
            var i = 0;
            foreach (var device in worstTenDevices)
            {
                i++;
                sb.Append($"{i}. {device} had {invalidCounters[device]} invalid packets");
            }
            log.LogInformation(sb.ToString());
        }
        private static async Task<Dictionary<string, long>> CountPackets(CloudTableClient client, string tableName)
        {
            var table = client.GetTableReference(tableName);
            var query = new TableQuery<TelemetryEntity>();
            TableContinuationToken continuationToken = null;
            var counterByDevice = new Dictionary<string, long>();
            do
            {
                var entries = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                foreach (var entry in entries)
                {
                    var deviceId = entry.PartitionKey;
                    long counter = 0;
                    if (!counterByDevice.TryGetValue(deviceId, out counter))
                    {
                        counterByDevice.Add(deviceId, counter);
                    }
                    counter++;
                    counterByDevice[deviceId] = counter;
                }
                continuationToken = entries.ContinuationToken;
            } while (continuationToken != null);
            return counterByDevice;
        }

    }

    public static class MergeDictionaries
    {
        public static void Merge(this IDictionary<string, long> first, IDictionary<string, long> second)
        {
            if (second == null || first == null) return;
            foreach (var item in second)
            {
                if (!first.ContainsKey(item.Key))
                {
                    first.Add(item.Key, item.Value);
                }
                else
                {
                    first[item.Key] += item.Value;
                }
            }
        }
    }
}

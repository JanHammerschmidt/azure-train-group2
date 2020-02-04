using System;
using Microsoft.Azure.CosmosDB.Table;

namespace api
{
    public class SensorValue : TableEntity
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public string Summary { get; set; }
        public string Id { get; set; }
        public string DeviceId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace api
{

    public class SensorValue : TableEntity
    {
        public long Temperature { get; set; }
        public long humidity { get; set; }
        public long windDirection { get; set; }
        public string id { get; set; }
        public string deviceId { get; set; }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            Temperature = properties["temperature"].Int64Value ?? 0;
            // TODO: nach mir die sintflut!
        }
    }
}

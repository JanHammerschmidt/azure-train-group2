using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CommaSoft.Gruppe2
{
    public static class TimerTriggerMidNightFunk
    {
        [FunctionName("TimerTriggerMidNightFunk")]
        public static void Run([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
        }
    }
}

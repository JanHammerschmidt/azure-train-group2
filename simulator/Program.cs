using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
// using Microsoft.Extensions.Configuration;


namespace simulator
{
    class SimulatedDevice
    {
        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private static string s_connectionString = "HostName=rg2-iothub.azure-devices.net;SharedAccessKey=PNtNXhORLmykHEPHDcYlDLrMaCN5ynYf/a9MaGZjAaE=";
        private static bool PushInvalidData = true;

        // Async method to send simulated telemetry
        private static async void SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            Random rand = new Random();

            while (true)
            {
                int currentTemperature = -50 + (int)(rand.NextDouble() * 100);
                if (PushInvalidData)
                {
                    currentTemperature += 50;
                }
                int currentWindSpeed = (int)(rand.NextDouble() * 70);
                int currentWindDirection = (int)(rand.NextDouble() * 359);
                int currentHumidity = (int)(rand.NextDouble() * 100);

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity,
                    windDirection = currentWindDirection,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                // Send the telemetry message
                await s_deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(500);
            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device. Ctrl-C to exit.\n");

            var deviceId = Environment.GetEnvironmentVariable("DEVICE_ID");
            if (deviceId == null)
            {
                deviceId = "testdevice1";
            }

            var pushInvalidData = Environment.GetEnvironmentVariable("PUSH_INVALID_DATA");
            if (pushInvalidData != null)
            {
                PushInvalidData = bool.Parse(pushInvalidData);
            }

            // Connect to the IoT hub using the MQTT protocol
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, deviceId, TransportType.Mqtt);
            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }
    }
}
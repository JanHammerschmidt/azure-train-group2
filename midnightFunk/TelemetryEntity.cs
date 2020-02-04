using Microsoft.WindowsAzure.Storage.Table;

public class TelemetryEntity : TableEntity
{
    public int Humidity { get; set; }
    public int Temperature { get; set; }
    public int WindDirection { get; set; }
    public int WindSpeed { get; set; }
}
namespace IOT.Data;

public class AzureBlobSettings
{
    public required string ConnectionString { get; set; }
    public required string ContainerName { get; set; }
}
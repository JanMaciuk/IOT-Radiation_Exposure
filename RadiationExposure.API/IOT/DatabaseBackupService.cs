using Azure.Storage.Blobs;
using IOT.Data;
using Microsoft.Extensions.Options;

namespace IOT;

public class DatabaseBackupService
{
    private readonly AzureBlobSettings _blobSettings;
    
    public DatabaseBackupService(IOptions<AzureBlobSettings> blobOptions)
    {
        _blobSettings = blobOptions.Value;
    }
    
    public async Task CreateDbBackupAsync()
    {
        var backupPath = Path.Combine(Directory.GetCurrentDirectory(), "iot_backup.db");
        File.Copy("iot.db", backupPath, true);
        await using var fileStream = new FileStream(backupPath, FileMode.Open);
        
        var blobServiceClient = new BlobServiceClient(_blobSettings.ConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);
        string blobName = $"backup-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.db";
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        
        await blobClient.UploadAsync(fileStream, true);
    }
}
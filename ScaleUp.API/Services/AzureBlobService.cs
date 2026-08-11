//namespace ScaleUp.API.Services;

//public class AzureBlobService
//{
//    private readonly BlobContainerClient _containerClient;

//    public AzureBlobService(IConfiguration config)
//    {
//        var connectionString = config["AzureBlob:ConnectionString"];
//        var containerName = config["AzureBlob:ContainerName"];

//        _containerClient = new BlobContainerClient(connectionString, containerName);
//        _containerClient.CreateIfNotExists();
//    }

//    public async Task<string> UploadVideoAsync(IFormFile file, string folder)
//    {
//        string extension = Path.GetExtension(file.FileName).ToLower();

//        // Allow only MP4
//        if (extension != ".mp4")
//            throw new Exception("Only .mp4 videos are allowed.");

//        string fileName = $"{Guid.NewGuid()}{extension}";
//        string blobPath = $"{folder}/{fileName}";

//        BlobClient blob = _containerClient.GetBlobClient(blobPath);

//        using (var stream = file.OpenReadStream())
//        {
//            await blob.UploadAsync(stream, overwrite: true);
//        }

//        return blob.Uri.ToString();
//    }

//    public async Task RenameFolderAsync(string oldFolder, string newFolder)
//    {
//        await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: oldFolder))
//        {
//            string oldPath = blobItem.Name;
//            string newPath = oldPath.Replace(oldFolder, newFolder);

//            BlobClient oldBlob = _containerClient.GetBlobClient(oldPath);
//            BlobClient newBlob = _containerClient.GetBlobClient(newPath);

//            await newBlob.StartCopyFromUriAsync(oldBlob.Uri);
//            await oldBlob.DeleteIfExistsAsync();
//        }
//    }
//}


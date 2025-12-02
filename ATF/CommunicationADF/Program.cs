using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Files.DataLake;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("error: CommunicationADF.exe <action> <parameters> - need action PLUS parameters");
            return;
        }
        string accountName = "";
        string accountKey = "";
        string fileSystemName = "";

        
        accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");
        if (accountName == "")
        {
            Console.WriteLine("error: Environment variable 'AZURE_STORAGE_ACCOUNT_NAME' is not set.");
            return;
        }
        if (accountKey == "")
        {
            Console.WriteLine("error: Environment variable 'AZURE_STORAGE_ACCOUNT_KEY' is not set.");
            return;
        }
        if (fileSystemName == "")
        {
            Console.WriteLine("error: Environment variable 'AZURE_STORAGE_FILESYSTEM' is not set.");
            return;
        }

        string action = args[0].ToLower();
        string[] parameters = args[1..];

        string response = action switch
        {
            "upload" => await UploadFile(parameters),
            "download" => await DownloadFile(parameters),
            "list" => await ListFiles(parameters),
            "delete" => await DeleteFile(parameters),
            "deletedirectory" => await DeleteDirectoryAndContents(parameters),
            "exist" => await DoesDirectoryExist(parameters),
            _ => "error: Invalid action. Use 'upload', 'download', or 'list'."
        };

        Console.WriteLine(response);
    }

    static async Task<string> DoesDirectoryExist(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return "error Usage: doesdirectoryexist <directoryPath>";
        }

        string accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        string accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        string fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");

        string directoryPath = parameters[0];

        try
        {
            var serviceClient = GetDataLakeServiceClient(accountName, accountKey);
            var fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            var directoryClient = fileSystemClient.GetDirectoryClient(directoryPath);

            var exists = await directoryClient.ExistsAsync();
            return exists.Value ? "true" : "error Directory does not exist.";
        }
        catch (Exception ex)
        {
            return $"error checking directory existence: {ex.Message}";
        }
    }
    
    static async Task<string> DeleteFile(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return "error Usage: delete <filePath>";
        }

        string accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        string accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        string fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");

        string filePath = parameters[0];

        try
        {
            var serviceClient = GetDataLakeServiceClient(accountName, accountKey);
            var fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            var fileClient = fileSystemClient.GetFileClient(filePath);

            await fileClient.DeleteIfExistsAsync();

            return "File deleted successfully.";
        }
        catch (Exception ex)
        {
            return $"error deleting file: {ex.Message}";
        }
    }

    static async Task<string> UploadFile(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return "error Usage: upload <filePath> <destinationPath>";
        }

        string accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        string accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        string fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");

        string filePath = parameters[0];
        string destinationPath = parameters.Length > 1 ? parameters[1] : Path.GetFileName(filePath);

        try
        {
            var serviceClient = GetDataLakeServiceClient(accountName, accountKey);
            var fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            var fileClient = fileSystemClient.GetFileClient(destinationPath);

            using var fileStream = File.OpenRead(filePath);
            await fileClient.UploadAsync(fileStream, overwrite: true);

            return "File uploaded successfully.";
        }
        catch (Exception ex)
        {
            return $"error uploading file: {ex.Message}";
        }
    }

    static async Task<string> DownloadFile(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return "error Usage: download <filePath> <destinationPath>";
        }

        string accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        string accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        string fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");

        string filePath = parameters[0];
        string destinationPath = parameters.Length > 1 ? parameters[1] : Path.GetFileName(filePath);

        try
        {
            var serviceClient = GetDataLakeServiceClient(accountName, accountKey);
            var fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            var fileClient = fileSystemClient.GetFileClient(filePath);

            var downloadResponse = await fileClient.ReadAsync();
            using var fileStream = File.Create(destinationPath);
            await downloadResponse.Value.Content.CopyToAsync(fileStream);

            return "File downloaded successfully.";
        }
        catch (Exception ex)
        {
            return $"error downloading file: {ex.Message}";
        }
    }


    static async Task<string> DeleteDirectoryAndContents(string[] parameters)
    {
        if (parameters.Length < 1)
        {
            return "error Usage: deletedirectory <directoryPath>";
        }

        string accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        string accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        string fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");

        string directoryPath = parameters[0];

        try
        {
            var serviceClient = GetDataLakeServiceClient(accountName, accountKey);
            var fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            var directoryClient = fileSystemClient.GetDirectoryClient(directoryPath);

            // Check if the directory exists
            var exists = await directoryClient.ExistsAsync();
            if (!exists.Value)
            {
                return "Directory does not exist.";
            }

            // Recursively delete all files and subdirectories
            await foreach (var pathItem in directoryClient.GetPathsAsync(recursive: true))
            {
                if (pathItem.IsDirectory == true)
                {
                    var subDirectoryClient = fileSystemClient.GetDirectoryClient(pathItem.Name);
                    await subDirectoryClient.DeleteIfExistsAsync();
                }
                else
                {
                    var fileClient = fileSystemClient.GetFileClient(pathItem.Name);
                    await fileClient.DeleteIfExistsAsync();
                }
            }
            // Delete the directory itself
            await directoryClient.DeleteIfExistsAsync();
            return "Directory and all contents deleted successfully.";
        }
        catch (Exception ex)
        {
            return $"error: deleting directory: {ex.Message}";
        }
    }


    static async Task<string> ListFiles(string[] parameters)
    {
        string accountName = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_NAME");
        string accountKey = GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT_KEY");
        string fileSystemName = GetEnvironmentVariable("AZURE_STORAGE_FILESYSTEM");

        string directoryPath = parameters.Length > 0 ? parameters[0] : string.Empty;

        try
        {
            var serviceClient = GetDataLakeServiceClient(accountName, accountKey);
            var fileSystemClient = serviceClient.GetFileSystemClient(fileSystemName);
            var directoryClient = fileSystemClient.GetDirectoryClient(directoryPath);

            var paths = directoryClient.GetPathsAsync();
            await foreach (var pathItem in paths)
            {
                Console.WriteLine(pathItem.Name);
            }

            return "";
        }
        catch (Exception ex)
        {
            return $"error listing files: {ex.Message}";
        }
    }

    static DataLakeServiceClient GetDataLakeServiceClient(string accountName, string accountKey)
    {
        string dfsUri = $"https://{accountName}.dfs.core.windows.net";
        var sharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);
        return new DataLakeServiceClient(new Uri(dfsUri), sharedKeyCredential);
    }

    static string GetEnvironmentVariable(string variableName)
    {
        string? value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception($"error Environment variable '{variableName}' is not set.");
        }
        return value;
    }
}
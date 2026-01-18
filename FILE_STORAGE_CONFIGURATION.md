# File Storage Configuration Guide

## Overview

The Marketing Platform provides flexible file storage options with support for multiple storage providers:

1. **Local File Storage (Default)** - Files stored on the server's local filesystem
2. **Azure Blob Storage (Optional)** - Microsoft Azure cloud storage
3. **AWS S3 (Optional)** - Amazon Web Services S3 storage

**Important**: Local storage is the default and always available. Cloud storage providers are optional and must be explicitly configured.

---

## 1. Storage Provider Configuration

### Configuration Structure

File storage is configured in `appsettings.json`:

```json
{
  "FileStorage": {
    "Provider": "Local",
    "Local": {
      "BasePath": "uploads"
    },
    "Azure": {
      "ConnectionString": "",
      "ContainerName": ""
    },
    "S3": {
      "AccessKey": "",
      "SecretKey": "",
      "Region": "us-east-1",
      "BucketName": ""
    }
  }
}
```

### Switching Providers

Change the `Provider` value to activate a different storage backend:
- `"Local"` - Local filesystem storage (default)
- `"Azure"` - Azure Blob Storage
- `"S3"` - AWS S3 Storage

---

## 2. Local File Storage (Default)

### Configuration

```json
{
  "FileStorage": {
    "Provider": "Local",
    "Local": {
      "BasePath": "uploads"
    }
  }
}
```

### Settings

- **BasePath**: Directory path for storing files (relative or absolute)
  - Relative path: `"uploads"` → `{app-root}/uploads`
  - Absolute path: `"/var/app/uploads"` → `/var/app/uploads`

### Features

- ✅ No additional setup required
- ✅ No external dependencies
- ✅ Fast for small files
- ✅ Free (uses server disk space)
- ⚠️ Not scalable for multiple servers
- ⚠️ Requires disk space management
- ⚠️ No built-in redundancy

### Usage Example

```csharp
// Files are automatically stored in the configured BasePath
// Example: uploads/templates/guid_template.docx
await _fileStorageService.UploadFileAsync(
    stream, 
    "template.docx", 
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "templates"
);
```

### File Access

Files are served through the application at `/uploads/{filePath}`. Ensure static file middleware is configured:

```csharp
// In Program.cs (if not already present)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});
```

### Backup Considerations

- Implement regular backup of the uploads directory
- Consider setting up file system monitoring
- Disk space monitoring recommended

---

## 3. Azure Blob Storage (Optional)

### Prerequisites

1. Azure subscription
2. Storage account created
3. Container created
4. Connection string or SAS token

### Configuration

```json
{
  "FileStorage": {
    "Provider": "Azure",
    "Azure": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=yourkey;EndpointSuffix=core.windows.net",
      "ContainerName": "marketing-files"
    }
  }
}
```

### Settings

- **ConnectionString**: Azure Storage connection string
  - Find in Azure Portal → Storage Account → Access Keys
  - Format: `DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...`
- **ContainerName**: Name of the blob container
  - Must be lowercase, 3-63 characters
  - Only letters, numbers, and hyphens

### Azure Portal Setup

1. **Create Storage Account**:
   - Navigate to Azure Portal
   - Create Resource → Storage Account
   - Choose performance tier (Standard/Premium)
   - Select redundancy option (LRS, GRS, etc.)

2. **Create Container**:
   - Open Storage Account
   - Containers → + Container
   - Name: `marketing-files`
   - Public access level: Private

3. **Get Connection String**:
   - Storage Account → Access Keys
   - Copy "Connection string" under key1 or key2

4. **Update Configuration**:
   - Add connection string to appsettings.json
   - Or use environment variable: `FileStorage__Azure__ConnectionString`
   - **Never commit secrets to source control**

### Features

- ✅ Highly scalable
- ✅ Geo-redundant options
- ✅ Built-in CDN integration
- ✅ Automatic backups
- ✅ SAS token support for secure access
- ✅ Works with multiple app servers
- 💰 Pay-per-use pricing
- ⚠️ Requires Azure subscription

### Security

- Store connection strings in Azure Key Vault
- Use Managed Identity when running in Azure
- Enable soft delete for accidental deletion protection
- Configure firewall rules to restrict access

### Cost Optimization

- Use cool or archive tier for infrequently accessed files
- Set lifecycle policies to automatically archive old files
- Monitor usage through Azure Cost Management

---

## 4. AWS S3 Storage (Optional)

### Prerequisites

1. AWS account
2. S3 bucket created
3. IAM user with S3 access
4. Access Key and Secret Key

### Configuration

```json
{
  "FileStorage": {
    "Provider": "S3",
    "S3": {
      "AccessKey": "AKIAIOSFODNN7EXAMPLE",
      "SecretKey": "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY",
      "Region": "us-east-1",
      "BucketName": "marketing-platform-files"
    }
  }
}
```

### Settings

- **AccessKey**: AWS IAM Access Key ID
- **SecretKey**: AWS IAM Secret Access Key
- **Region**: AWS region where bucket is located
  - Examples: `us-east-1`, `eu-west-1`, `ap-southeast-1`
- **BucketName**: Name of the S3 bucket
  - Must be globally unique
  - 3-63 characters, lowercase

### AWS Console Setup

1. **Create S3 Bucket**:
   - Navigate to AWS S3 Console
   - Click "Create bucket"
   - Choose unique bucket name: `marketing-platform-files`
   - Select region (e.g., `us-east-1`)
   - Block all public access (recommended)
   - Enable versioning (optional)
   - Create bucket

2. **Create IAM User**:
   - Navigate to IAM Console
   - Users → Add user
   - Name: `marketing-platform-storage`
   - Access type: Programmatic access
   - Attach policy: `AmazonS3FullAccess` (or create custom policy)

3. **Custom IAM Policy** (Recommended):
   ```json
   {
     "Version": "2012-10-17",
     "Statement": [
       {
         "Effect": "Allow",
         "Action": [
           "s3:PutObject",
           "s3:GetObject",
           "s3:DeleteObject",
           "s3:ListBucket"
         ],
         "Resource": [
           "arn:aws:s3:::marketing-platform-files",
           "arn:aws:s3:::marketing-platform-files/*"
         ]
       }
     ]
   }
   ```

4. **Get Credentials**:
   - After creating user, download credentials CSV
   - Copy Access Key ID and Secret Access Key
   - **Store securely - cannot retrieve later**

5. **Update Configuration**:
   - Add credentials to appsettings.json
   - Or use environment variables:
     - `FileStorage__S3__AccessKey`
     - `FileStorage__S3__SecretKey`
   - **Never commit secrets to source control**

### Features

- ✅ Highly scalable and durable (99.999999999% durability)
- ✅ Global CDN with CloudFront
- ✅ Versioning and lifecycle policies
- ✅ Cross-region replication
- ✅ Pre-signed URLs for temporary access
- ✅ Works with multiple app servers
- 💰 Pay-per-use pricing
- ⚠️ Requires AWS account

### Security Best Practices

- Use IAM roles instead of access keys when running on AWS (EC2, ECS, Lambda)
- Enable bucket encryption
- Enable access logging
- Use AWS Secrets Manager for credential storage
- Implement least privilege IAM policies
- Enable MFA delete for added protection

### Cost Optimization

- Use S3 Intelligent-Tiering for automatic cost optimization
- Set lifecycle rules to move old files to Glacier
- Enable S3 Transfer Acceleration only if needed
- Monitor costs with AWS Cost Explorer
- Use S3 Storage Class Analysis

---

## 5. File Storage Service Usage

### IFileStorageService Interface

```csharp
public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? folder = null);
    Task<Stream> DownloadFileAsync(string fileKey);
    Task<bool> DeleteFileAsync(string fileKey);
    Task<bool> FileExistsAsync(string fileKey);
    Task<string> GetFileUrlAsync(string fileKey, int expiryMinutes = 60);
    Task<IEnumerable<string>> ListFilesAsync(string? folder = null);
}
```

### Upload File Example

```csharp
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FileController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        using var stream = file.OpenReadStream();
        var fileKey = await _fileStorageService.UploadFileAsync(
            stream,
            file.FileName,
            file.ContentType,
            "uploads/templates"
        );

        return Ok(new { fileKey });
    }
}
```

### Download File Example

```csharp
[HttpGet("download/{fileKey}")]
public async Task<IActionResult> DownloadFile(string fileKey)
{
    if (!await _fileStorageService.FileExistsAsync(fileKey))
        return NotFound();

    var stream = await _fileStorageService.DownloadFileAsync(fileKey);
    return File(stream, "application/octet-stream", Path.GetFileName(fileKey));
}
```

### Get File URL Example

```csharp
[HttpGet("url/{fileKey}")]
public async Task<IActionResult> GetFileUrl(string fileKey)
{
    var url = await _fileStorageService.GetFileUrlAsync(fileKey, expiryMinutes: 60);
    return Ok(new { url });
}
```

### Delete File Example

```csharp
[HttpDelete("{fileKey}")]
public async Task<IActionResult> DeleteFile(string fileKey)
{
    var result = await _fileStorageService.DeleteFileAsync(fileKey);
    return result ? Ok() : NotFound();
}
```

---

## 6. Migration Between Providers

### From Local to Azure/S3

1. **Update Configuration**:
   ```json
   {
     "FileStorage": {
       "Provider": "Azure"
     }
   }
   ```

2. **Upload Existing Files**:
   ```csharp
   public async Task MigrateFiles()
   {
       var localFiles = Directory.GetFiles("uploads", "*", SearchOption.AllDirectories);
       
       foreach (var file in localFiles)
       {
           using var stream = File.OpenRead(file);
           var relativePath = Path.GetRelativePath("uploads", file);
           await _fileStorageService.UploadFileAsync(
               stream,
               Path.GetFileName(file),
               "application/octet-stream",
               Path.GetDirectoryName(relativePath)
           );
       }
   }
   ```

3. **Test Thoroughly** before removing local files

4. **Update Database** if file paths are stored

### From Azure/S3 to Another Provider

Similar process - enumerate files, download, and upload to new provider.

---

## 7. Environment-Specific Configuration

### Development
```json
{
  "FileStorage": {
    "Provider": "Local",
    "Local": {
      "BasePath": "uploads"
    }
  }
}
```

### Staging/Production (Azure)
```json
{
  "FileStorage": {
    "Provider": "Azure",
    "Azure": {
      "ConnectionString": "#{AzureStorageConnectionString}#",
      "ContainerName": "marketing-files-prod"
    }
  }
}
```

### Using Environment Variables

```bash
# Linux/Mac
export FileStorage__Provider="Azure"
export FileStorage__Azure__ConnectionString="your-connection-string"
export FileStorage__Azure__ContainerName="marketing-files"

# Windows PowerShell
$env:FileStorage__Provider="Azure"
$env:FileStorage__Azure__ConnectionString="your-connection-string"
$env:FileStorage__Azure__ContainerName="marketing-files"
```

### Using Azure Key Vault

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential()
);
```

Store as secrets:
- `FileStorage--Azure--ConnectionString`
- `FileStorage--S3--AccessKey`
- `FileStorage--S3--SecretKey`

---

## 8. Troubleshooting

### Local Storage Issues

**Problem**: "Directory not found"
- **Solution**: Ensure BasePath exists or app has permission to create it

**Problem**: "Access denied"
- **Solution**: Check file system permissions for the application user

### Azure Blob Storage Issues

**Problem**: "Connection string is invalid"
- **Solution**: Verify connection string format and credentials

**Problem**: "Container not found"
- **Solution**: Ensure container exists and name is correct (lowercase)

**Problem**: "Insufficient permissions"
- **Solution**: Check SAS token permissions or storage account access policies

### AWS S3 Issues

**Problem**: "The AWS Access Key Id you provided does not exist"
- **Solution**: Verify AccessKey and SecretKey are correct

**Problem**: "Access Denied"
- **Solution**: Check IAM user permissions, ensure s3:PutObject, s3:GetObject permissions

**Problem**: "Bucket does not exist"
- **Solution**: Verify bucket name and region are correct

---

## 9. Best Practices

### Security
- ✅ Never commit credentials to source control
- ✅ Use environment variables or secret management systems
- ✅ Enable encryption at rest (Azure/S3)
- ✅ Use HTTPS for all transfers
- ✅ Implement access controls and signed URLs
- ✅ Regularly rotate access keys
- ✅ Enable audit logging

### Performance
- ✅ Use CDN for frequently accessed files
- ✅ Implement caching strategies
- ✅ Consider file size limits
- ✅ Use streaming for large files
- ✅ Implement pagination for file listings

### Cost Management
- ✅ Monitor storage usage and costs
- ✅ Implement lifecycle policies
- ✅ Delete temporary files regularly
- ✅ Use appropriate storage tiers
- ✅ Set up cost alerts

### Reliability
- ✅ Implement retry logic for transient failures
- ✅ Use redundant storage options
- ✅ Regular backups (even for cloud storage)
- ✅ Monitor storage health
- ✅ Test disaster recovery procedures

---

## 10. Summary

| Feature | Local | Azure Blob | AWS S3 |
|---------|-------|------------|--------|
| Setup Complexity | ⭐ Easy | ⭐⭐ Medium | ⭐⭐ Medium |
| Scalability | ⭐ Limited | ⭐⭐⭐ Excellent | ⭐⭐⭐ Excellent |
| Cost | Free (disk) | Pay-per-use | Pay-per-use |
| Redundancy | None | Built-in | Built-in |
| Global CDN | No | Yes | Yes |
| Multi-server | No | Yes | Yes |
| **Recommended For** | Development | Production (Azure) | Production (AWS) |

**Default Choice**: Start with Local storage for development, migrate to Azure or S3 for production deployments with multiple servers or high availability requirements.

---

For additional support or questions, contact your system administrator or refer to the main documentation.

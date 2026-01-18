# Data Encryption Implementation - Task 14.3

## Overview
This document describes the comprehensive data encryption implementation for the Marketing Platform, ensuring all sensitive data is protected both in transit and at rest using industry-standard encryption methods.

## Encryption Standards

### Data at Rest
- **Algorithm**: AES-256-CBC (Advanced Encryption Standard with 256-bit keys in Cipher Block Chaining mode)
- **Key Size**: 256 bits (32 bytes)
- **Padding**: PKCS7
- **IV**: Randomly generated for each encryption operation (16 bytes)
- **Format**: `{keyVersion}:{IV}:{EncryptedData}` (all Base64 encoded)

### Data in Transit
- **Protocol**: TLS 1.2 or higher (TLS 1.3 recommended)
- **HTTPS**: Enforced on all API endpoints
- **HSTS**: HTTP Strict Transport Security enabled with 1-year max-age
- **Certificate**: SSL/TLS certificates required for production

## Architecture

### Core Components

#### 1. IEncryptionService
Interface for encryption/decryption operations.
```csharp
public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
    Task<string> EncryptAsync(string plainText);
    Task<string> DecryptAsync(string encryptedText);
}
```

**Implementation**: `AesEncryptionService`
- Location: `MarketingPlatform.Infrastructure/Services/AesEncryptionService.cs`
- Uses AES-256-CBC for encryption
- Generates unique IV for each encryption
- Supports key versioning for rotation

#### 2. IKeyManagementService
Interface for managing encryption keys with support for multiple providers.
```csharp
public interface IKeyManagementService
{
    Task<byte[]> GetEncryptionKeyAsync();
    Task<byte[]> GetEncryptionKeyAsync(string keyVersion);
    Task<string> RotateKeyAsync();
    Task<string> GetCurrentKeyVersionAsync();
}
```

**Implementations**:
- `ConfigurationKeyManagementService` (Default) - Stores keys in configuration
- `AzureKeyVaultService` (Optional) - Integrates with Azure Key Vault
- `AwsKmsService` (Optional) - Integrates with AWS KMS

#### 3. Repository Pattern with Encryption
Transparent encryption layer using the repository pattern:
- `EncryptedRepository<T>` - Base class for encrypted repositories
- `ContactEncryptedRepository` - Encrypts Contact PII fields

### Encrypted Fields

#### Contact Entity
The following Contact fields are encrypted at rest:
- `Email` - Personal email address
- `PhoneNumber` - Contact phone number
- `CustomAttributes` - JSON containing custom contact attributes

#### Future Extensions
Additional entities that should have encryption:
- MessageTemplate content fields
- API credentials in MessageProvider
- OAuth tokens in RefreshToken
- Sensitive configuration values

## Configuration

### appsettings.json

```json
{
  "Encryption": {
    "Provider": "Configuration",
    "CurrentKeyVersion": "v1",
    "Keys": {
      "v1": "BASE64_ENCODED_256_BIT_KEY"
    },
    "Azure": {
      "KeyVaultUrl": "",
      "KeyName": "",
      "TenantId": "",
      "ClientId": "",
      "ClientSecret": ""
    },
    "AWS": {
      "Region": "",
      "KeyId": "",
      "AccessKeyId": "",
      "SecretAccessKey": ""
    }
  },
  "Security": {
    "EnforceTLS": true,
    "MinimumTLSVersion": "1.2",
    "EnableHSTS": true,
    "HSTSMaxAge": 31536000,
    "IncludeSubDomains": true
  }
}
```

### Key Provider Configuration

#### Configuration Provider (Default)
Set `Encryption:Provider` to `"Configuration"`:
- Keys stored in appsettings.json or environment variables
- Suitable for development and small deployments
- **Security Note**: In production, use environment variables or secure configuration providers, NOT appsettings.json

#### Azure Key Vault Provider
Set `Encryption:Provider` to `"Azure"`:
- Requires: `Azure.Security.KeyVault.Keys` NuGet package
- Keys stored in Azure Key Vault
- Automatic key rotation support
- Managed Service Identity (MSI) support

Configuration:
```json
"Azure": {
  "KeyVaultUrl": "https://your-vault.vault.azure.net/",
  "KeyName": "marketing-platform-encryption-key",
  "TenantId": "your-tenant-id",
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret"
}
```

#### AWS KMS Provider
Set `Encryption:Provider` to `"AWS"`:
- Requires: `AWSSDK.KeyManagementService` NuGet package
- Keys stored in AWS Key Management Service
- Automatic key rotation support
- IAM role support

Configuration:
```json
"AWS": {
  "Region": "us-east-1",
  "KeyId": "arn:aws:kms:region:account:key/key-id",
  "AccessKeyId": "your-access-key",
  "SecretAccessKey": "your-secret-key"
}
```

## Key Generation

### Generate New Encryption Key

#### Using C#:
```csharp
using System.Security.Cryptography;

var key = new byte[32]; // 256 bits
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(key);
}
var keyBase64 = Convert.ToBase64String(key);
Console.WriteLine(keyBase64);
```

#### Using Python:
```python
import secrets
import base64
key = secrets.token_bytes(32)
print(base64.b64encode(key).decode())
```

#### Using OpenSSL:
```bash
openssl rand -base64 32
```

#### Using .NET CLI:
```bash
dotnet run --project MarketingPlatform.Infrastructure -- GenerateKey
```

**IMPORTANT**: 
- Store generated keys securely
- Never commit keys to source control
- Use environment variables or Key Vault in production
- Keep backup of keys in secure offline storage

## Key Rotation

### Process

1. **Generate New Key**:
   ```bash
   # Generate new 256-bit key
   openssl rand -base64 32
   ```

2. **Add to Configuration**:
   ```json
   "Keys": {
     "v1": "OLD_KEY_BASE64",
     "v2": "NEW_KEY_BASE64"
   }
   ```

3. **Update Current Version**:
   ```json
   "CurrentKeyVersion": "v2"
   ```

4. **Data Re-encryption** (Optional but recommended):
   - New data automatically uses v2
   - Old data can still be decrypted using v1
   - Run re-encryption job to update old data:
     ```bash
     dotnet run --project MarketingPlatform.API -- ReEncryptData
     ```

5. **Verify**: Test encryption/decryption with both keys

6. **Retire Old Key** (After grace period):
   - Ensure all data is re-encrypted with new key
   - Remove old key from configuration
   - Document retirement in audit logs

### Best Practices
- Rotate keys annually or when compromised
- Maintain 2-3 key versions during rotation
- Keep audit trail of all rotations
- Test decryption before removing old keys
- Use automated rotation with Azure/AWS

## TLS/HTTPS Configuration

### Enforcement
The application enforces HTTPS/TLS through:

1. **HTTPS Redirection**: All HTTP requests redirected to HTTPS
2. **HSTS Headers**: Browsers forced to use HTTPS for 1 year
3. **Security Headers**: Additional security headers added

### Program.cs Configuration
```csharp
// HTTPS Redirection
app.UseHttpsRedirection();

// HSTS Headers
app.UseHsts();

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});
```

### Production SSL/TLS Setup

#### IIS/Windows Server:
1. Obtain SSL certificate from trusted CA (Let's Encrypt, DigiCert, etc.)
2. Import certificate to Windows Certificate Store
3. Bind certificate to IIS site
4. Configure SSL settings: TLS 1.2+ only
5. Disable weak cipher suites

#### Kestrel (Linux/Docker):
```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });
});
```

#### Reverse Proxy (Nginx):
```nginx
server {
    listen 443 ssl http2;
    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;
    
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
}
```

## Security Audit & Compliance

### Encryption Audit Logging
All encryption operations are logged in `EncryptionAuditLog` table:
- Operation type (Encrypt/Decrypt/KeyRotation)
- Entity type and ID
- Field name
- Key version used
- Success/failure status
- User and IP address
- Timestamp

### Regular Audits
Perform these audits quarterly:

1. **Key Review**:
   - Verify keys are stored securely
   - Check key rotation schedule
   - Validate key strength (256-bit)

2. **Encryption Coverage**:
   - Verify all PII fields encrypted
   - Check for new sensitive fields
   - Review encryption audit logs

3. **TLS Configuration**:
   - Verify TLS 1.2+ enforcement
   - Check certificate validity
   - Test HSTS headers
   - Review cipher suites

4. **Compliance Check**:
   - GDPR: Encryption of personal data ✓
   - HIPAA: Encryption at rest and in transit ✓
   - PCI DSS: Strong cryptography ✓
   - SOC 2: Encryption controls ✓

### Audit Commands
```bash
# Check TLS version
openssl s_client -connect your-domain.com:443 -tls1_2

# Verify HSTS headers
curl -I https://your-domain.com

# Test encryption/decryption
dotnet test MarketingPlatform.Tests --filter Category=Encryption

# Review audit logs
SELECT * FROM EncryptionAuditLogs 
WHERE OperationTimestamp > DATEADD(day, -30, GETDATE())
ORDER BY OperationTimestamp DESC;
```

## Migration from Unencrypted Data

### Step-by-Step Migration

1. **Backup Database**:
   ```sql
   BACKUP DATABASE MarketingPlatformDb 
   TO DISK = 'C:\Backups\pre-encryption-backup.bak'
   ```

2. **Add Encryption Infrastructure**:
   - Deploy encryption services
   - Configure encryption keys
   - Run database migration for EncryptionAuditLog table

3. **Test on Staging**:
   - Encrypt sample data
   - Verify decryption works
   - Test application functionality

4. **Production Migration**:
   ```bash
   dotnet run --project MarketingPlatform.API -- MigrateToEncryption
   ```

5. **Verify Migration**:
   - Check all records encrypted
   - Test data retrieval
   - Review audit logs

6. **Monitor**:
   - Watch for decryption errors
   - Monitor performance impact
   - Review error logs

### Migration Script Example
```csharp
// Encrypt all existing Contact records
var contacts = await _contactRepository.GetAllAsync();
foreach (var contact in contacts)
{
    if (!IsEncrypted(contact.Email))
        contact.Email = _encryptionService.Encrypt(contact.Email);
    
    if (!IsEncrypted(contact.PhoneNumber))
        contact.PhoneNumber = _encryptionService.Encrypt(contact.PhoneNumber);
    
    _contactRepository.Update(contact);
}
await _unitOfWork.SaveChangesAsync();
```

## Performance Considerations

### Encryption Overhead
- AES-256 encryption: ~0.1-0.5ms per field
- Negligible impact for typical API operations
- Batch operations may see 5-10% performance decrease

### Optimization Strategies
1. **Caching**: Cache decrypted data in memory (with appropriate TTL)
2. **Async Operations**: Use async encryption for large batches
3. **Selective Encryption**: Only encrypt truly sensitive fields
4. **Database Indexing**: Cannot index encrypted fields directly
   - Use hashed values for searchable encrypted fields
   - Implement secure search patterns

### Monitoring
Monitor these metrics:
- Encryption operation duration
- Failed encryption/decryption attempts
- Key retrieval performance
- TLS handshake time

## Troubleshooting

### Common Issues

#### "Encryption key not found"
**Solution**: Verify `Encryption:Keys:v1` exists in configuration

#### "Failed to decrypt data"
**Causes**:
1. Wrong key version
2. Corrupted encrypted data
3. Legacy unencrypted data

**Solution**: Check EncryptionAuditLog for key version used during encryption

#### "TLS handshake failed"
**Solution**: Verify certificate is valid and TLS 1.2+ is enabled

#### Performance Degradation
**Solution**: 
1. Check key retrieval performance
2. Implement caching for encryption service
3. Use async operations for batch processing

## References

### Standards & Compliance
- NIST SP 800-38A: AES Block Cipher Modes
- NIST SP 800-57: Key Management
- GDPR Article 32: Security of Processing
- HIPAA Security Rule: Encryption Standards
- PCI DSS Requirement 3: Protect Stored Cardholder Data

### Related Documentation
- [Repository Pattern Verification](../REPOSITORY_PATTERN_VERIFICATION.md)
- [API Integration Documentation](../API_INTEGRATION_DOCUMENTATION.md)
- [Compliance Implementation](../COMPLIANCE_IMPLEMENTATION_SUMMARY.md)

### External Resources
- [Azure Key Vault Documentation](https://docs.microsoft.com/azure/key-vault/)
- [AWS KMS Documentation](https://docs.aws.amazon.com/kms/)
- [.NET Cryptography Guidelines](https://docs.microsoft.com/dotnet/standard/security/cryptography-model)
- [OWASP Cryptographic Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)

## Maintenance Schedule

### Daily
- Monitor encryption audit logs
- Check for failed encryption operations

### Weekly
- Review security headers
- Verify HTTPS/TLS enforcement

### Monthly
- Review encryption coverage
- Check certificate expiration
- Analyze performance metrics

### Quarterly
- Full security audit
- Test key rotation procedure
- Update documentation

### Annually
- Rotate encryption keys
- Review and update encryption policies
- Compliance assessment

## Contact & Support

For questions or issues related to encryption:
1. Review this documentation
2. Check EncryptionAuditLog for operation details
3. Contact security team: security@marketingplatform.com
4. Escalate to infrastructure team for key management issues

---

**Last Updated**: 2026-01-18
**Version**: 1.0
**Author**: Marketing Platform Security Team

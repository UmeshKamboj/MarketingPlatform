using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Interfaces;
using MarketingPlatform.Core.Interfaces.Repositories;

namespace MarketingPlatform.Infrastructure.Repositories
{
    /// <summary>
    /// Encrypted repository for Contact entities.
    /// Automatically encrypts sensitive fields (Email, PhoneNumber, CustomAttributes) at rest.
    /// </summary>
    public class ContactEncryptedRepository : EncryptedRepository<Contact>
    {
        private readonly IEncryptionService _encryptionService;

        public ContactEncryptedRepository(
            IRepository<Contact> innerRepository,
            IEncryptionService encryptionService,
            IServiceProvider serviceProvider)
            : base(innerRepository, encryptionService, serviceProvider)
        {
            _encryptionService = encryptionService;
        }

        protected override void EncryptEntity(Contact contact)
        {
            if (contact == null) return;

            // Encrypt sensitive PII fields
            if (!string.IsNullOrEmpty(contact.Email) && !IsEncrypted(contact.Email))
            {
                contact.Email = _encryptionService.Encrypt(contact.Email);
            }

            if (!string.IsNullOrEmpty(contact.PhoneNumber) && !IsEncrypted(contact.PhoneNumber))
            {
                contact.PhoneNumber = _encryptionService.Encrypt(contact.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(contact.CustomAttributes) && !IsEncrypted(contact.CustomAttributes))
            {
                contact.CustomAttributes = _encryptionService.Encrypt(contact.CustomAttributes);
            }
        }

        protected override void DecryptEntity(Contact contact)
        {
            if (contact == null) return;

            // Decrypt sensitive PII fields
            if (!string.IsNullOrEmpty(contact.Email) && IsEncrypted(contact.Email))
            {
                try
                {
                    contact.Email = _encryptionService.Decrypt(contact.Email);
                }
                catch
                {
                    // If decryption fails, field might not be encrypted (legacy data)
                    // Leave as is and log warning in production
                }
            }

            if (!string.IsNullOrEmpty(contact.PhoneNumber) && IsEncrypted(contact.PhoneNumber))
            {
                try
                {
                    contact.PhoneNumber = _encryptionService.Decrypt(contact.PhoneNumber);
                }
                catch
                {
                    // If decryption fails, field might not be encrypted (legacy data)
                }
            }

            if (!string.IsNullOrEmpty(contact.CustomAttributes) && IsEncrypted(contact.CustomAttributes))
            {
                try
                {
                    contact.CustomAttributes = _encryptionService.Decrypt(contact.CustomAttributes);
                }
                catch
                {
                    // If decryption fails, field might not be encrypted (legacy data)
                }
            }
        }

        /// <summary>
        /// Checks if a value is already encrypted by looking for the version:IV:data format.
        /// </summary>
        private bool IsEncrypted(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // Check for encryption format: version:IV:encryptedData
            var parts = value.Split(':');
            return parts.Length == 3 && parts[0].StartsWith("v");
        }
    }
}

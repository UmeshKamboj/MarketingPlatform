using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using MarketingPlatform.Application.DTOs.Common;
using MarketingPlatform.Application.DTOs.Contact;
using MarketingPlatform.Application.Interfaces;
using MarketingPlatform.Core.Entities;
using MarketingPlatform.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;
using System.Text;

namespace MarketingPlatform.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<ContactGroupMember> _groupMemberRepository;
        private readonly IRepository<ContactTagAssignment> _tagRepository;
        private readonly IRepository<ContactGroup> _contactGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactService> _logger;

        public ContactService(
            IRepository<Contact> contactRepository,
            IRepository<ContactGroupMember> groupMemberRepository,
            IRepository<ContactTagAssignment> tagRepository,
            IRepository<ContactGroup> contactGroupRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ContactService> logger)
        {
            _contactRepository = contactRepository;
            _groupMemberRepository = groupMemberRepository;
            _tagRepository = tagRepository;
            _contactGroupRepository = contactGroupRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<ContactDto?> GetContactByIdAsync(string userId, int contactId)
        {
            var contact = await _contactRepository.FirstOrDefaultAsync(c => 
                c.Id == contactId && c.UserId == userId && !c.IsDeleted);

            if (contact == null)
                return null;

            var dto = _mapper.Map<ContactDto>(contact);
            dto.CustomAttributes = DeserializeCustomAttributes(contact.CustomAttributes);

            // Load groups
            var groupMembers = await _groupMemberRepository.FindAsync(gm => 
                gm.ContactId == contactId && !gm.IsDeleted);
            var groupIds = groupMembers.Select(gm => gm.ContactGroupId).ToList();
            var groups = await _contactGroupRepository.FindAsync(g => 
                groupIds.Contains(g.Id) && !g.IsDeleted);
            dto.Groups = groups.Select(g => g.Name).ToList();

            // Load tags
            var tagAssignments = await _tagRepository.FindAsync(ta => 
                ta.ContactId == contactId && !ta.IsDeleted);
            dto.Tags = tagAssignments.Select(ta => ta.ContactTagId.ToString()).ToList();

            return dto;
        }

        public async Task<PaginatedResult<ContactDto>> GetContactsAsync(string userId, PagedRequest request)
        {
            var query = (await _contactRepository.FindAsync(c => 
                c.UserId == userId && !c.IsDeleted)).AsQueryable();

            // Apply search
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(c =>
                    (c.Email != null && c.Email.ToLower().Contains(searchLower)) ||
                    c.PhoneNumber.ToLower().Contains(searchLower) ||
                    (c.FirstName != null && c.FirstName.ToLower().Contains(searchLower)) ||
                    (c.LastName != null && c.LastName.ToLower().Contains(searchLower)));
            }

            var totalCount = query.Count();

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = request.SortBy.ToLower() switch
                {
                    "email" => request.SortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                    "firstname" => request.SortDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),
                    "lastname" => request.SortDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
                    "createdat" => request.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            // Apply pagination
            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var dtos = new List<ContactDto>();
            foreach (var contact in items)
            {
                var dto = _mapper.Map<ContactDto>(contact);
                dto.CustomAttributes = DeserializeCustomAttributes(contact.CustomAttributes);
                dtos.Add(dto);
            }

            return new PaginatedResult<ContactDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }

        public async Task<ContactDto> CreateContactAsync(string userId, CreateContactDto dto)
        {
            var contact = _mapper.Map<Contact>(dto);
            contact.UserId = userId;
            contact.CustomAttributes = SerializeCustomAttributes(dto.CustomAttributes);

            await _contactRepository.AddAsync(contact);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ContactDto>(contact);
            result.CustomAttributes = dto.CustomAttributes;
            return result;
        }

        public async Task<bool> UpdateContactAsync(string userId, int contactId, UpdateContactDto dto)
        {
            var contact = await _contactRepository.FirstOrDefaultAsync(c => 
                c.Id == contactId && c.UserId == userId && !c.IsDeleted);

            if (contact == null)
                return false;

            contact.PhoneNumber = dto.PhoneNumber ?? contact.PhoneNumber;
            contact.Email = dto.Email ?? contact.Email;
            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            contact.Country = dto.Country;
            contact.City = dto.City;
            contact.PostalCode = dto.PostalCode;
            contact.IsActive = dto.IsActive;
            contact.CustomAttributes = SerializeCustomAttributes(dto.CustomAttributes);
            contact.UpdatedAt = DateTime.UtcNow;

            _contactRepository.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteContactAsync(string userId, int contactId)
        {
            var contact = await _contactRepository.FirstOrDefaultAsync(c => 
                c.Id == contactId && c.UserId == userId && !c.IsDeleted);

            if (contact == null)
                return false;

            contact.IsDeleted = true;
            contact.UpdatedAt = DateTime.UtcNow;

            _contactRepository.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteContactsAsync(string userId, List<int> contactIds)
        {
            var contacts = await _contactRepository.FindAsync(c => 
                contactIds.Contains(c.Id) && c.UserId == userId && !c.IsDeleted);

            if (!contacts.Any())
                return false;

            foreach (var contact in contacts)
            {
                contact.IsDeleted = true;
                contact.UpdatedAt = DateTime.UtcNow;
            }

            _contactRepository.UpdateRange(contacts);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ContactImportResultDto> ImportContactsFromCsvAsync(string userId, IFormFile file, int? groupId = null)
        {
            var result = new ContactImportResultDto();

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null
                });

                var records = csv.GetRecords<ContactImportDto>().ToList();
                result.TotalRows = records.Count;

                foreach (var record in records)
                {
                    try
                    {
                        // Check for duplicate
                        var isDuplicate = await _contactRepository.AnyAsync(c =>
                            c.UserId == userId &&
                            !c.IsDeleted &&
                            ((c.Email != null && record.Email != null && c.Email == record.Email) ||
                             (c.PhoneNumber == record.PhoneNumber && !string.IsNullOrEmpty(record.PhoneNumber))));

                        if (isDuplicate)
                        {
                            result.DuplicateCount++;
                            continue;
                        }

                        var contact = new Contact
                        {
                            UserId = userId,
                            PhoneNumber = record.PhoneNumber ?? string.Empty,
                            Email = record.Email,
                            FirstName = record.FirstName,
                            LastName = record.LastName,
                            Country = record.Country,
                            City = record.City,
                            PostalCode = record.PostalCode,
                            IsActive = true
                        };

                        await _contactRepository.AddAsync(contact);
                        await _unitOfWork.SaveChangesAsync();

                        // Add to group if specified
                        if (groupId.HasValue)
                        {
                            var groupMember = new ContactGroupMember
                            {
                                ContactId = contact.Id,
                                ContactGroupId = groupId.Value,
                                JoinedAt = DateTime.UtcNow
                            };
                            await _groupMemberRepository.AddAsync(groupMember);
                            
                            // Update group contact count
                            var group = await _contactGroupRepository.GetByIdAsync(groupId.Value);
                            if (group != null)
                            {
                                group.ContactCount++;
                                _contactGroupRepository.Update(group);
                            }
                        }

                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Row error: {ex.Message}");
                        _logger.LogError(ex, "Error importing contact");
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add($"File processing error: {ex.Message}");
                _logger.LogError(ex, "Error processing CSV file");
            }

            return result;
        }

        public async Task<ContactImportResultDto> ImportContactsFromExcelAsync(string userId, IFormFile file, int? groupId = null)
        {
            var result = new ContactImportResultDto();

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                result.TotalRows = rowCount - 1; // Exclude header

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var phoneNumber = worksheet.Cells[row, 1].Value?.ToString();
                        var email = worksheet.Cells[row, 2].Value?.ToString();
                        var firstName = worksheet.Cells[row, 3].Value?.ToString();
                        var lastName = worksheet.Cells[row, 4].Value?.ToString();
                        var country = worksheet.Cells[row, 5].Value?.ToString();
                        var city = worksheet.Cells[row, 6].Value?.ToString();
                        var postalCode = worksheet.Cells[row, 7].Value?.ToString();

                        // Check for duplicate
                        var isDuplicate = await _contactRepository.AnyAsync(c =>
                            c.UserId == userId &&
                            !c.IsDeleted &&
                            ((c.Email != null && email != null && c.Email == email) ||
                             (c.PhoneNumber == phoneNumber && !string.IsNullOrEmpty(phoneNumber))));

                        if (isDuplicate)
                        {
                            result.DuplicateCount++;
                            continue;
                        }

                        var contact = new Contact
                        {
                            UserId = userId,
                            PhoneNumber = phoneNumber ?? string.Empty,
                            Email = email,
                            FirstName = firstName,
                            LastName = lastName,
                            Country = country,
                            City = city,
                            PostalCode = postalCode,
                            IsActive = true
                        };

                        await _contactRepository.AddAsync(contact);
                        await _unitOfWork.SaveChangesAsync();

                        // Add to group if specified
                        if (groupId.HasValue)
                        {
                            var groupMember = new ContactGroupMember
                            {
                                ContactId = contact.Id,
                                ContactGroupId = groupId.Value,
                                JoinedAt = DateTime.UtcNow
                            };
                            await _groupMemberRepository.AddAsync(groupMember);
                            
                            // Update group contact count
                            var group = await _contactGroupRepository.GetByIdAsync(groupId.Value);
                            if (group != null)
                            {
                                group.ContactCount++;
                                _contactGroupRepository.Update(group);
                            }
                        }

                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Row {row} error: {ex.Message}");
                        _logger.LogError(ex, $"Error importing contact at row {row}");
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add($"File processing error: {ex.Message}");
                _logger.LogError(ex, "Error processing Excel file");
            }

            return result;
        }

        public async Task<byte[]> ExportContactsToCsvAsync(string userId, List<int>? contactIds = null)
        {
            IEnumerable<Contact> contacts;

            if (contactIds != null && contactIds.Any())
            {
                contacts = await _contactRepository.FindAsync(c => 
                    contactIds.Contains(c.Id) && c.UserId == userId && !c.IsDeleted);
            }
            else
            {
                contacts = await _contactRepository.FindAsync(c => 
                    c.UserId == userId && !c.IsDeleted);
            }

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteField("PhoneNumber");
            csv.WriteField("Email");
            csv.WriteField("FirstName");
            csv.WriteField("LastName");
            csv.WriteField("Country");
            csv.WriteField("City");
            csv.WriteField("PostalCode");
            csv.WriteField("CreatedAt");
            csv.NextRecord();

            foreach (var contact in contacts)
            {
                csv.WriteField(contact.PhoneNumber);
                csv.WriteField(contact.Email);
                csv.WriteField(contact.FirstName);
                csv.WriteField(contact.LastName);
                csv.WriteField(contact.Country);
                csv.WriteField(contact.City);
                csv.WriteField(contact.PostalCode);
                csv.WriteField(contact.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                csv.NextRecord();
            }

            writer.Flush();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportContactsToExcelAsync(string userId, List<int>? contactIds = null)
        {
            IEnumerable<Contact> contacts;

            if (contactIds != null && contactIds.Any())
            {
                contacts = await _contactRepository.FindAsync(c => 
                    contactIds.Contains(c.Id) && c.UserId == userId && !c.IsDeleted);
            }
            else
            {
                contacts = await _contactRepository.FindAsync(c => 
                    c.UserId == userId && !c.IsDeleted);
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Contacts");

            // Headers
            worksheet.Cells[1, 1].Value = "PhoneNumber";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "FirstName";
            worksheet.Cells[1, 4].Value = "LastName";
            worksheet.Cells[1, 5].Value = "Country";
            worksheet.Cells[1, 6].Value = "City";
            worksheet.Cells[1, 7].Value = "PostalCode";
            worksheet.Cells[1, 8].Value = "CreatedAt";

            // Data
            int row = 2;
            foreach (var contact in contacts)
            {
                worksheet.Cells[row, 1].Value = contact.PhoneNumber;
                worksheet.Cells[row, 2].Value = contact.Email;
                worksheet.Cells[row, 3].Value = contact.FirstName;
                worksheet.Cells[row, 4].Value = contact.LastName;
                worksheet.Cells[row, 5].Value = contact.Country;
                worksheet.Cells[row, 6].Value = contact.City;
                worksheet.Cells[row, 7].Value = contact.PostalCode;
                worksheet.Cells[row, 8].Value = contact.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            return package.GetAsByteArray();
        }

        public async Task<List<ContactDto>> SearchContactsAsync(string userId, string searchTerm)
        {
            var searchLower = searchTerm.ToLower();
            var contacts = await _contactRepository.FindAsync(c =>
                c.UserId == userId &&
                !c.IsDeleted &&
                ((c.Email != null && c.Email.ToLower().Contains(searchLower)) ||
                 c.PhoneNumber.ToLower().Contains(searchLower) ||
                 (c.FirstName != null && c.FirstName.ToLower().Contains(searchLower)) ||
                 (c.LastName != null && c.LastName.ToLower().Contains(searchLower))));

            var dtos = new List<ContactDto>();
            foreach (var contact in contacts)
            {
                var dto = _mapper.Map<ContactDto>(contact);
                dto.CustomAttributes = DeserializeCustomAttributes(contact.CustomAttributes);
                dtos.Add(dto);
            }

            return dtos;
        }

        private string? SerializeCustomAttributes(Dictionary<string, string>? attributes)
        {
            if (attributes == null || !attributes.Any())
                return null;

            return JsonConvert.SerializeObject(attributes);
        }

        private Dictionary<string, string>? DeserializeCustomAttributes(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}

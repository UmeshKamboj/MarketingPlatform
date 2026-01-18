using AutoMapper;
using MarketingPlatform.Application.DTOs.User;
using MarketingPlatform.Application.DTOs.Contact;
using MarketingPlatform.Application.DTOs.ContactGroup;
using MarketingPlatform.Application.DTOs.Campaign;
using MarketingPlatform.Application.DTOs.Template;
using MarketingPlatform.Application.DTOs.Message;
using MarketingPlatform.Application.DTOs.SuppressionList;
using MarketingPlatform.Application.DTOs.ContactTag;
using MarketingPlatform.Core.Entities;

namespace MarketingPlatform.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UpdateUserDto, ApplicationUser>();

            // Contact mappings
            CreateMap<Contact, ContactDto>();
            CreateMap<CreateContactDto, Contact>();
            CreateMap<UpdateContactDto, Contact>();

            // ContactGroup mappings
            CreateMap<ContactGroup, ContactGroupDto>();
            CreateMap<CreateContactGroupDto, ContactGroup>();

            // Campaign mappings
            CreateMap<Campaign, CampaignDto>();
            CreateMap<CreateCampaignDto, Campaign>();
            CreateMap<UpdateCampaignDto, Campaign>();
            CreateMap<CampaignContent, CampaignContentDto>();
            CreateMap<CampaignAudience, CampaignAudienceDto>();
            CreateMap<CampaignSchedule, CampaignScheduleDto>();

            // Template mappings
            CreateMap<MessageTemplate, TemplateDto>();
            CreateMap<CreateTemplateDto, MessageTemplate>();
            CreateMap<UpdateTemplateDto, MessageTemplate>();
            // Message mappings
            CreateMap<CampaignMessage, MessageDto>()
                .ForMember(dest => dest.CampaignName, opt => opt.MapFrom(src => src.Campaign.Name))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => 
                    src.Contact.FirstName + " " + src.Contact.LastName))
                .ForMember(dest => dest.MediaUrls, opt => opt.Ignore()); // Handled manually in service
            CreateMap<CreateMessageDto, CampaignMessage>()
                .ForMember(dest => dest.MediaUrls, opt => opt.Ignore()); // Handled manually in service

            // SuppressionList mappings
            CreateMap<SuppressionList, SuppressionListDto>();
            CreateMap<CreateSuppressionListDto, SuppressionList>();

            // ContactTag mappings
            CreateMap<ContactTag, ContactTagDto>()
                .ForMember(dest => dest.ContactCount, opt => opt.Ignore()); // Handled manually in service
            CreateMap<CreateContactTagDto, ContactTag>();
        }
    }
}

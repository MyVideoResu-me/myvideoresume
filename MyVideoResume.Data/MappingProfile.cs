using AutoMapper;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Productivity;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Productivity;

namespace MyVideoResume.Mapper;

public partial class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserProfileDTO, UserProfileEntity>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<JSONResume, ExportJSONResume>();
        CreateMap<Basics, ExportBasics>();
        CreateMap<Work, ExportWork>();
        CreateMap<Volunteer, ExportVolunteer>();
        CreateMap<Education, ExportEducation>();
        CreateMap<Award, ExportAward>();
        CreateMap<Certificate, ExportCertificate>();
        CreateMap<Publication, ExportPublication>();
        CreateMap<Skill, ExportSkill>();
        CreateMap<LanguageItem, ExportLanguageItem>();
        CreateMap<Interest, ExportInterest>();
        CreateMap<ReferenceItem, ExportReferenceItem>();
        CreateMap<Project, ExportProject>();
        CreateMap<Location, ExportLocation>();

        // Task mappings
        CreateMap<TaskDTO, TaskEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.CreatedByUserId) ? (Guid?)null :
                ParseGuid(src.CreatedByUserId)))
            .ForMember(dest => dest.AssignedToUserId, opt => opt.MapFrom(src =>
                src.AssignedToUserId ?? (Guid?)null));

        CreateMap<TaskEntity, TaskDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src =>
                src.CreatedByUserId.HasValue ? src.CreatedByUserId.Value.ToString() : null))
            .ForMember(dest => dest.AssignedToUserId, opt => opt.MapFrom(src =>
                src.AssignedToUserId));
    }

    private static Guid? ParseGuid(string? guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            return guid;
        }
        return null;
    }
}

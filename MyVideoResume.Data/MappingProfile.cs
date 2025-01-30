using AutoMapper;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.Data.Models.Account.Profiles;

namespace MyVideoResume.Mapper;

public partial class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserProfileDTO, UserProfileEntity>();
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
        
    }
}

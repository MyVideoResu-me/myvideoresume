using AutoMapper;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;

namespace MyVideoResume.Mapper;

public partial class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<JSONResume, ExportJSONResume>().ReverseMap();
        CreateMap<ExportJSONResume, JSONResume>();
        CreateMap<JSONResume, ExportJSONResume>();
    }
}

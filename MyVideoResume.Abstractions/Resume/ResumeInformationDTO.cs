using MyVideoResume.Abstractions.MetaContent;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using System.Text.Json.Serialization;

namespace MyVideoResume.Abstractions.Resume;

public class ResumeInformationDTO : ResumeInformation, IResumeItem
{
    public string Id { get; set; }
    public JSONResumeDTO MetaResume { get; set; }
    public List<MetaDataDTO> MetaData { get; set; }

    public ResumeTemplate? ResumeTemplate { get; set; }

    [JsonIgnore]
    public Dictionary<string, MetaDataDTO> MetaDataLookup
    {
        get
        {
            if (MetaData != null)
                return MetaData.ToDictionary(K => K.ReferenceId, Y => Y);
            else
                return new Dictionary<string, MetaDataDTO>();
        }
    }
}

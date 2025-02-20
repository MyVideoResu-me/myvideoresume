using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyVideoResume.Abstractions.Resume;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using MyVideoResume.Data.Models.MetaContent;
using MyVideoResume.Data.Models.Account.Profiles;

namespace MyVideoResume.Data.Models.Resume;

[Table("ResumeInformation")]
public class ResumeInformationEntity : ResumeInformation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [InverseProperty("ResumeInformation")]
    public MetaResumeEntity MetaResume { get; set; }

    [JsonIgnore]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity? UserProfile { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<MetaDataEntity> MetaData { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public ResumeTemplateEntity? ResumeTemplate { get; set; }

    [JsonIgnore]
    public Dictionary<string, MetaDataEntity> MetaDataLookup
    {
        get
        {
            if (MetaData != null)
                return MetaData.ToDictionary(K => K.ReferenceId, Y => Y);
            else
                return new Dictionary<string, MetaDataEntity>();
        }
    }

}

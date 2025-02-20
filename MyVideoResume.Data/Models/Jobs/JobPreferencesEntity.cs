using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyVideoResume.Abstractions;
using MyVideoResume.Abstractions.Job;

namespace MyVideoResume.Data.Models.Jobs;

[Table("JobPreferences")]
public class JobPreferencesEntity : JobPreferences
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}

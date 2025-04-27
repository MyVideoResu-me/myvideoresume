using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyVideoResume.Abstractions;
using MyVideoResume.Abstractions.Account.Preferences;

namespace MyVideoResume.Data.Models.Account.Preferences;

[Table("JobPreferences")]
public class JobPreferencesEntity : JobPreferences
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Abstractions.Productivity;

namespace MyVideoResume.Data.Models.Productivity;

[Table("Projects")]
public class ProjectEntity: ProjectGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity UserProfile { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? CompanyProfile { get; set; }

    public List<TaskEntity>? ToDos { get; set; }
}

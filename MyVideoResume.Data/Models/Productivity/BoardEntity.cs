using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Abstractions.Productivity;

namespace MyVideoResume.Data.Models.Productivity;

[Table("Boards")]
public class BoardEntity : Board
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity CreatedByUser { get; set; } //System Created Tasks will not have an assigned created by User

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? CompanyProfile { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<UserProfileEntity> SharedWithUsers { get; set; } //System Created Tasks will not have an assigned created by User

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ProjectEntity> Projects { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<TaskEntity> Tasks { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<AppointmentEntity> Appointments { get; set; }

}


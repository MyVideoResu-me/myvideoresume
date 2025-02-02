using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Abstractions.Business.Tasks;
using MyVideoResume.Abstractions.Business;

namespace MyVideoResume.Data.Models.Business.Tasks;

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
    public List<TaskEntity> Tasks { get; set; }
}

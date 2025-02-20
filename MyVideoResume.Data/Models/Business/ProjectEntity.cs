using MyVideoResume.Abstractions.Business;
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
using MyVideoResume.Data.Models.Business.Tasks;

namespace MyVideoResume.Data.Models.Business;

[Table("Projects")]
public class ProjectEntity: Project
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

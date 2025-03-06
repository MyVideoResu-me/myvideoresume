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

namespace MyVideoResume.Data.Models.Business.Tasks;

[Table("Tasks")]
public class TaskEntity : TaskItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [ForeignKey("AssignedToUserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity AssignedToUser { get; set; }
    public Guid AssignedToUserId { get; set; }

    [ForeignKey("CreatedByUserId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity? CreatedByUser { get; set; }
    public Guid? CreatedByUserId { get; set; }

    [ForeignKey("CompanyProfileId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? CompanyProfile { get; set; }
    public Guid? CompanyProfileId { get; set; }
}

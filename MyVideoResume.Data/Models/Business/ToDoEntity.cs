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
using MyVideoResume.Data.Models.Profiles;

namespace MyVideoResume.Data.Models.Business;

[Table("Todos")]
public class TodoEntity: Todo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity CreatedByUser{ get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity AssignedToUser { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? CompanyProfile { get; set; }

}

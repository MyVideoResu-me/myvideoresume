using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Data.Models.Profiles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models.Jobs;

[Table("Jobs")]
public class JobItemEntity : JobItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity? CreatedByUser { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public UserProfileEntity? ContactUser { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CompanyProfileEntity? Company { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public AddressEntity? Address { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public BonusEntity? Bonus { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public EquityEntity? Equity { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public SalaryEntity? Salary { get; set; }
}

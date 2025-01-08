using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Data.Models.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models.Jobs;

[Table("Jobs")]
public class JobItemEntity : JobItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public CompanyProfileEntity? Company { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public AddressEntity? Address { get; set; }

    public BonusEntity? Bonus { get; set; }
    public EquityEntity? Equity { get; set; }

    public SalaryEntity? Salary { get; set; }
}

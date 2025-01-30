using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Profiles;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Profiles;

[Table("UserProfiles")]
public class UserProfileEntity : UserProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public JobPreferencesEntity JobPreferences { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ResumeInformationEntity> ResumeItems { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ApplicantToJobEntity> JobApplications { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public AddressEntity? MailingAddress { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public AddressEntity? BillingAddress { get; set; }

}

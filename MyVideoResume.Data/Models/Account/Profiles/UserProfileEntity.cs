using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Data.Models.Account.Profiles;

[Table("UserProfiles")]
public class UserProfileEntity : UserProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<Phone>? Phones { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<Email>? Emails { get; set; }


    [DeleteBehavior(DeleteBehavior.NoAction)]
    public JobPreferencesEntity JobPreferences { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ResumeInformationEntity> ResumeItems { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<ApplicantToJobEntity> JobApplications { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<AddressEntity>? MailingAddresses { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public AddressEntity? BillingAddress { get; set; }

}

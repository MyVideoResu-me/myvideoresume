using Microsoft.Extensions.Logging;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Business;

namespace MyVideoResume.Application;

public class AccountService
{

    private readonly DataContext _dataContext;

    private readonly ILogger<AccountService> logger;


    public AccountService(DataContext dataContextService, ILogger<AccountService> logger)
    {
        this._dataContext = dataContextService;
        this.logger = logger;
    }

    public async Task<UserProfileEntity> CreateUserProfile(string userId)
    {
        var profile = new UserProfileEntity();
        try
        {
            profile = _dataContext.UserProfiles.FirstOrDefault(x => x.UserId == userId);
            if (profile == null)
            {
                var jobPreferences = new JobPreferencesEntity() { UserId = userId, CreationDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow };
                _dataContext.JobPreferences.Add(jobPreferences);
                profile = new UserProfileEntity() { FirstName = string.Empty, LastName = String.Empty, UserId = userId, CreationDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, JobPreferences = jobPreferences };
                _dataContext.UserProfiles.Add(profile);
                await _dataContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw;
        }

        return profile;
    }
    public async Task<CompanyProfileEntity> CreateCompanyProfile(string userId, UserProfileEntity userOwner)
    {
        var profile = new CompanyProfileEntity();
        try
        {
            profile = _dataContext.CompanyProfiles.FirstOrDefault(x => x.UserId == userId);
            if (profile == null)
            {
                var dateTime = DateTime.UtcNow;

                //Create the Address
                var addressEntity = new AddressEntity() { Country = string.Empty, Line1 = string.Empty, Line2 = string.Empty, PostalZipCode = string.Empty, StateProvince = string.Empty, City = string.Empty, CreationDateTime = dateTime, UserId = userId };
                _dataContext.Addresses.Add(addressEntity);

                //Create the Company Profile
                profile = new CompanyProfileEntity() { UserProfile = userOwner, Name = string.Empty, UserId = userId, CreationDateTime = dateTime, UpdateDateTime = dateTime, BillingAddress = addressEntity, MailingAddress = addressEntity, TermsOfUseAgreementAcceptedDateTime = DateTime.UtcNow, TermsOfUserAgreementVersion = "2024.11.10" };
                _dataContext.CompanyProfiles.Add(profile);

                await _dataContext.SaveChangesAsync();

                //Associate the User to the Company and give the user Owner Rights
                var userCompanyRoleAssociation = new UserCompanyRoleEntity() { UserProfile = userOwner, CompanyProfile = profile, RolesAssigned = new List<MyVideoResumeRoles> { MyVideoResumeRoles.AccountAdmin, MyVideoResumeRoles.AccountOwner } };
                _dataContext.UserCompanyRoles.Add(userCompanyRoleAssociation);
                await _dataContext.SaveChangesAsync();

            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw;
        }

        return profile;
    }


}

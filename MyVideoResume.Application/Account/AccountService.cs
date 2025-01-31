using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Jobs;

namespace MyVideoResume.Application.Account;

public class AccountService
{

    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AccountService(DataContext dataContextService, ILogger<AccountService> logger, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        this._dataContext = dataContextService;
        this._logger = logger;
        this._mapper = mapper;
        this._userManager = userManager;
        this._roleManager = roleManager;
    }

    public async Task<List<string>> GetUserRoles(string userId)
    {
        var result  = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
        return result.ToList();
    }

    public async Task<ResponseResult<UserProfileDTO>> GetUserProfile(string userId)
    {
        var result = new ResponseResult<UserProfileDTO>();

        var userProfile = _dataContext.UserProfiles.Where(z => z.UserId == userId).Select(x => new UserProfileDTO() { Id = x.Id.ToString(), UserId = x.UserId, IsPaidAccount = x.IsPaidAccount, IsRoleSelected = x.IsRoleSelected, IsRoleSelectedDateTime = x.IsRoleSelectedDateTime, CreationDateTime = x.CreationDateTime, FirstName = x.FirstName, LastName = x.LastName, TermsOfUseAgreementAcceptedDateTime = x.TermsOfUseAgreementAcceptedDateTime, TermsOfUserAgreementVersion = x.TermsOfUserAgreementVersion, RoleSelected = x.RoleSelected }).FirstOrDefault();
        result.Result = userProfile;

        return result;
    }

    public async Task<ResponseResult<UserProfileDTO>> UpdateUserProfileRole(UserProfileDTO profileRequest, string userId)
    {
        var result = new ResponseResult<UserProfileDTO>();
        //Verify its the same USER...
        if (profileRequest.UserId == userId)
        {
            var userProfile = _dataContext.UserProfiles.Where(z => z.UserId == userId).FirstOrDefault();
            if (userProfile != null)
            {
                profileRequest.IsRoleSelected = true;
                profileRequest.IsRoleSelectedDateTime = DateTime.UtcNow;
                _mapper.Map(profileRequest, userProfile);
                _dataContext.UserProfiles.Update(userProfile);
                await _dataContext.SaveChangesAsync();

                //We need to assign the User the selected role.
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.RemoveFromRoleAsync(user, Enum.GetName(MyVideoResumeRoles.Recruiter));
                await _userManager.RemoveFromRoleAsync(user, Enum.GetName(MyVideoResumeRoles.JobSeeker));
                await _userManager.AddToRoleAsync(user, Enum.GetName(profileRequest.RoleSelected.Value));
                var userRoles = _userManager.GetRolesAsync(user);
            }
            result.Result = profileRequest;
        }

        return result;
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
                profile = new UserProfileEntity() { FirstName = string.Empty, LastName = string.Empty, UserId = userId, CreationDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, JobPreferences = jobPreferences };
                _dataContext.UserProfiles.Add(profile);
                await _dataContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
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
            _logger.LogError(ex.Message, ex);
            throw;
        }

        return profile;
    }


}

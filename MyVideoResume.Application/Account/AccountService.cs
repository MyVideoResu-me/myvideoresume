﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Application.Business;
using MyVideoResume.Data;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Data.Models.Account.Profiles;
using MyVideoResume.Data.Models.Business;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Server.Data;

namespace MyVideoResume.Application.Account;

public class AccountService
{

    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ILogger<AccountService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly TaskService _taskService;
    private readonly ApplicationIdentityDbContext _identityDbContext;

    public AccountService(DataContext dataContextService, ILogger<AccountService> logger, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, TaskService taskService)
    {
        this._dataContext = dataContextService;
        this._logger = logger;
        this._mapper = mapper;
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._taskService = taskService;
    }

    public async Task<ResponseResult<List<UserCompanyRoleAssociationEntity>>> GetCompanyUsers(string userId)
    {

        var result = new ResponseResult<List<UserCompanyRoleAssociationEntity>>();
        //based upon the logged in User get the Company Profile then get all the users ba
        var users = await _dataContext.CompanyUserAssociation.Where(z => z.UserId == userId).ToListAsync();

        if (users.Any())
        {
            result.Result = users;
        }
        return result;
    }

    public async Task<List<string>> GetUserRoles(string userId)
    {
        var result = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
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
                //var userRoles = _userManager.GetRolesAsync(user);

                //Add Tasks
                //await _taskService.UpdateTasksByRole();


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
                profile = new UserProfileEntity() { FirstName = string.Empty, SocialProfiles = new List<string>(), LastName = string.Empty, UserId = userId, CreationDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, JobPreferences = jobPreferences };
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

    public async Task<UserCompanyRoleAssociationEntity> CreateCompanyProfile(string userId, UserProfileEntity userProfile)
    {
        var association = new UserCompanyRoleAssociationEntity();
        try
        {
            //Check and see if there is an existing Company Association for this user
            association = _dataContext.CompanyUserAssociation.Include(x => x.UserProfile).FirstOrDefault(x => x.UserProfile.UserId == userId);

            if (association != null)
            {
                if (association.InviteStatus != InviteStatus.Owner)
                {
                    if (association.InviteStatus == InviteStatus.Invited)
                    {
                        association.InviteStatus = InviteStatus.Accepted;
                        association.UpdateDateTime = DateTime.UtcNow;
                        association.InviteStatusEndDateTime = DateTime.UtcNow;
                        _dataContext.CompanyUserAssociation.Update(association);
                    }
                }
            }

            var companyProfile = _dataContext.CompanyProfiles.FirstOrDefault(x => x.UserId == userId);
            if (companyProfile == null)
            {
                var dateTime = DateTime.UtcNow;

                //Create the Address
                var addressEntity = new AddressEntity() { Country = string.Empty, Line1 = string.Empty, Line2 = string.Empty, PostalZipCode = string.Empty, StateProvince = string.Empty, City = string.Empty, CreationDateTime = dateTime, UserId = userId };
                _dataContext.Addresses.Add(addressEntity);

                //Create the Company Profile
                companyProfile = new CompanyProfileEntity() { SocialProfiles = new List<string>(), UserProfile = userProfile, Name = string.Empty, UserId = userId, CreationDateTime = dateTime, UpdateDateTime = dateTime, BillingAddress = addressEntity, MailingAddress = addressEntity, TermsOfUseAgreementAcceptedDateTime = DateTime.UtcNow, TermsOfUserAgreementVersion = "2024.11.10" };
                _dataContext.CompanyProfiles.Add(companyProfile);
            }

            if (association == null && companyProfile != null)
            {
                //Now Create the association with the Company as the OWNER
                association = await this.CreateUserCompanyRoleAssociation(userId, userProfile, companyProfile, InviteStatus.Owner, new List<MyVideoResumeRoles> { MyVideoResumeRoles.AccountAdmin, MyVideoResumeRoles.AccountOwner }); //Create Owner Record.
            }

            //Check and see if this user is associated to the Company
            if (companyProfile.CompanyUsers == null || companyProfile.CompanyUsers.Count == 0) //First User association so lets create the array and add this user
            {
                companyProfile.CompanyUsers = new List<UserCompanyRoleAssociationEntity>();
                companyProfile.CompanyUsers.Add(association);
                _dataContext.CompanyProfiles.Update(companyProfile);
            }
            else //There are users associated; validate they don't exist and add them.
            {
                var found = companyProfile.CompanyUsers.FirstOrDefault(x => x.UserId == userProfile.UserId);
                if (found == null)
                {
                    companyProfile.CompanyUsers.Add(association);
                    _dataContext.CompanyProfiles.Update(companyProfile);
                }
            }

            await _dataContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }

        return association;
    }

    public async Task CreateAccount(string userId)
    {
        //Create the User's Profile if it doesn't exist
        var userProfile = await this.CreateUserProfile(userId);

        //Create a Company Profile and Associate it with the userProfile (checks to see if there is already a Company Role assignment invite; if it exists then return that company)
        var association = await this.CreateCompanyProfile(userId, userProfile);

        //Let's Create the Onboarding Tasks
        var tasks = await _taskService.CreateOnboardingTasks(userId, association.UserProfile, association.CompanyProfile);
    }

    public async Task<UserCompanyRoleAssociationEntity> CreateUserCompanyRoleAssociation(string userId, UserProfileEntity userProfile, CompanyProfileEntity companyProfile, InviteStatus status, List<MyVideoResumeRoles> roles)
    {
        var profile = new UserCompanyRoleAssociationEntity();
        try
        {
            profile = _dataContext.CompanyUserAssociation.Include(x => x.UserProfile).FirstOrDefault(x => x.UserProfile.UserId == userId && x.InviteStatus == status);

            if (profile == null)
            {
                var dateTime = DateTime.UtcNow;

                //Associate the User to the Company and give the user Owner Rights
                var association = profile = new UserCompanyRoleAssociationEntity() { CreationDateTime = DateTime.UtcNow, InviteStatusStartDateTime = DateTime.UtcNow, InviteStatus = status, UserId = userId, UserProfile = userProfile, CompanyProfile = companyProfile, RolesAssigned = roles };
                _dataContext.CompanyUserAssociation.Add(association);
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

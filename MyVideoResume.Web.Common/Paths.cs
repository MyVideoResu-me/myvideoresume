using Azure;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Data.Models.Resume;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace MyVideoResume.Web.Common;

public static class Paths
{
    public const string Site_Root = "/";
    public const string Site_Login = "/login";
    public const string Site_Logout = "/logout";
    public const string Site_About = "/about";
    public const string Site_TermsOfUse = "/termsofuse";
    public const string Site_PrivacyPolicy = "/privacypolicy";
    public const string Site_Help = "articles/how-to";
    public const string Site_ContactUs = "/work-with-us";
    public const string Site_ReleaseNotes = "/release-notes";
    public const string Site_Pricing = "/pricing";
    public const string Admin_BackgroundJobsPortal = "/Workers";
    public const string Admin_ApplicationUsers = "admin/application-users";
    public const string Admin_ApplicationRoles = "admin/application-roles";
    public const string Profile_Settings = "/profile/settings";
    public const string FeatureFlags_API = "api/featureflag";
    public const string Dashboard_View = "/dashboard";
    public const string Companies_View = "/companies";
    public const string Resume_View = "/resumes";
    public const string Resume_CreateNew = "resumes/builder/new";
    public const string Resume_Edit = "resumes/builder/";
    public const string Resume_API_CreateFromFile = "api/resume/createFromFile";
    public const string Resume_API_Save = "api/resume/save";
    public const string Resume_API_Parse = "api/resume/parse";
    public const string Resume_API_Sentiment = "api/resume/sentimentprediction";
    public const string Jobs_View = "/jobs";
    public const string Jobs_Edit = "/jobs/Edit";
    public const string Jobs_CreateNew = "/jobs/builder/new";
    public const string Jobs_API_View = "api/job/GetPublicJobs";
    public const string Jobs_API_CreateFromFile = "api/job/createFromFile";
    public const string Jobs_API_SummaryItems = "api/job/GetSummaryItems";
    public const string Jobs_API_ViewById = "api/job/{id}";
    public const string Jobs_API_CreateFromHtml = "api/job/CreateFromHtml";
    public const string Jobs_API_Save = "api/job/save";
    public const string Jobs_API_Extract = "api/job/extract";
    public const string AI_API_Sentiment = "sentiment/sentimentprediction";
    public const string Inbox_View = "/tasks";
    public const string Inbox_API_View = "api/inbox";
    public const string Tasks_View = "/tasks";
    public const string Tasks_Schedule = "/schedule";
    public const string Tasks_API_View = "api/tasks";
    public const string Tasks_API_Save = "api/tasks/save";
    public const string Tools_Sentiment = "Tools/SentimentAnalysis";
    public const string Tools_SummarizeResume = "Tools/SummarizeResume";
    public const string Tools_JobMatch = "Tools/JobResumeMatch";
    public const string Tools_BestResume = "Tools/JobResumeBestResume";
    public const string Tools_PdfToJson = "Tools/pdftojson";
}

public static class Constants
{
    public const string Recruiter = "Recruiter";
    public const string JobSeeker = "JobSeeker";
    public const string Admin = "Admin";
    public const string HttpClientFactory = "MyVideoResume.Server";
    public const string BaseUriConfigurationProperty = "BaseUri";
    public const string CampaignId = "Cam";
    public const string ReferrerUserId = "Ref";
}

public static class CacheKeys
{
    public const string UserProfile = "UserProfile";
    public const string UserRoles = "UserRoles";
}
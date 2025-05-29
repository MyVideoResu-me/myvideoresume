using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Core;


public enum DisplayPrivacy
{
    [Description("Visible to Public")]
    ToPublic = 0,
    [Description("Visible to Recruiters")]
    ToRecruiters = 1,
    [Description("Visible to Connections")]
    ToConnections = 2,
    [Description("Visible to Self (Private)")]
    ToSelf = 10
}

public enum MyVideoResumeRoles
{
    Admin = 0,
    AccountOwner = 10,
    AccountAdmin = 11,
    Employer = 20,
    EmployerManager = 21,
    EmployerEmployee = 22,
    EmployerExecutive = 23,
    JobSeeker = 30,
    Recruiter = 40,
}

public enum Actions
{
    ReadView,
    UpdateEdit,
    Create,
    Delete,
}

public enum CommunicationType { Personal, Work, Other }
public enum ContactType { Candidate, Contact, Contractor }

public enum Source
{
    [Description("Manual")]
    Manual = 0,
    [Description("3rd Party Firm")]
    ThirdPartyFirm = 1,
    [Description("AngelList")]
    AngelList = 2,
    [Description("Candidate Referral")]
    CandidateReferral = 3,
    [Description("Careerbuilder")]
    Careerbuilder = 4,
    [Description("Cold Call")]
    ColdCall = 5,
    [Description("Dice")]
    Dice = 6,
    [Description("Email")]
    Email = 7,
    [Description("Facebook")]
    Facebook = 8,
    [Description("GlassDoor")]
    GlassDoor = 9,
    [Description("Google")]
    Google = 10,
    [Description("Hubspot")]
    Hubspot = 11,
    [Description("Indeed")]
    Indeed = 12,
    [Description("LinkedIn")]
    LinkedIn = 13,
    [Description("Monster")]
    Monster = 14,
    [Description("Other")]
    Other = 15,
    [Description("Prior Candidate Relationship")]
    PriorCandidateRelationship = 16,
    [Description("Referral")]
    Referral = 17,
    [Description("Resume")]
    Resume = 18,
    [Description("SimplyHired")]
    SimplyHired = 19,
    [Description("TalentNexus")]
    TalentNexus = 20,
    [Description("X")]
    X = 21,
    [Description("Twitter")]
    Twitter = 22,
    [Description("ZipRecruiter")]
    ZipRecruiter = 23,
    [Description("Salesforce")]
    Salesforce = 24,
    [Description("MyVideoResume")]
    MyVideoResume = 25,
}


public enum InviteStatus
{
    Invited = 0,
    Accepted = 1,
    Rejected = 2,
    Resent = 3,
    Expired = 4,
    Owner = 5
}

public enum BatchProcessStatus
{
    NotStarted,
    Processing,
    Completed,
    Failed
}

public enum ActivityType
{
    [Description("Applied to Job")]
    JobApplied = 0,
    [Description("Added to Job")]
    AddedToJob = 1,
    [Description("Longlisted")]
    JobLonglisted = 2,
    [Description("Shortlisted")]
    JobShortlisted = 3,
    [Description("Marked as Maybe")]
    Maybe = 4,
    [Description("Note Updated")]
    NoteUpdated = 5,
    [Description("Marked a Yes")]
    MarkedAYes = 6,
    [Description("Left VoiceMail")] //Outbound
    LeftVoiceMail = 7,
    [Description("Sent Email")] //Outbound
    SentEmail = 8,
    [Description("Sent InMail")] //Outbound
    SentInMail = 9,
    [Description("Consultant Interview")] //Screening
    ConsultantInterview = 10,
    [Description("Submitted to Client")] //Screening
    ClientSubmitted = 11,
    [Description("1st Client Interview")] //Screening
    ClientInterviewOne = 12,
    [Description("2nd Client Interview")] //Screening
    ClientInterviewTwo = 13,
    [Description("3rd Client Interview")] //Screening
    ClientInterviewThree = 14,
    [Description("Final Client Interview")] //Screening
    ClientInterviewFinal = 15,
    [Description("Hold")]
    Hold = 16,
    [Description("Rejected")]
    Rejected = 17,
    [Description("Rejected by Candidate")]
    RejectedByCandidate = 18,
    [Description("Rejected by Client")]
    RejectedByClient = 19,
    [Description("Rejected by Consultant")]
    RejectedByConsultant = 20,
    [Description("Rejected by Automation AI")]
    RejectedByAutomationAI = 21,
    [Description("Hired")]
    Hired = 22,
    [Description("Form Filled")]
    FormFilled = 23,
    [Description("Hiring Manager commented (Maybe)")]
    HiringManagerMaybe = 24,
    [Description("Hiring Manager commented (No)")]
    HiringManagerNo = 25,
    [Description("Hiring Manager commented (Yes)")]
    HiringManagerYes = 26,
    [Description("Meeting Scheduled")]
    MeetingScheduled = 27,
    [Description("Offer Extended")]
    OfferExtended = 28,
    [Description("Scheduling")]
    Scheduling = 29,
    [Description("Scorecard Submitted")]
    ScorecardSubmitted = 29,
    [Description("Saved Job")] //Saved the Job
    JobSaved = 30,
    [Description("In Review")] //Reviewing
    ClientReviewing = 31, //Client Reviewing
    [Description("Job Closed")] //Job Closed
    JobClosed = 32, //
    [Description("Intake Note")] //Job Closed
    InTakeNote = 33, //

    [Description("Item in Planning")] //Planning
    TaskStatusPlanning = 50,
    [Description("Item in Todo")] //Todo 
    TaskStatusTodo = 51,
    [Description("Item open")] //
    TaskStatusOpen = 52,
    [Description("Item in progress")] //
    TaskStatusInprogress = 53,
    [Description("Item closed")] //
    TaskStatusClosed = 54,
    [Description("Item descoped")] //
    TaskStatusDescoped = 55,
    [Description("Task Type changed")] //
    TaskTypeChanged = 55,



    [Description("System Generated Message")]
    SystemGenerated = 500,
    [Description("System Error")]
    SystemError = 501,
    [Description("MyVideoResu.ME AI Sourced")]
    SystemSourced = 502,
    [Description("Sign In")]
    SystemLoggedIn = 503,
    [Description("Sign out")]
    SystemLoggedOut = 504,
}

public enum CommunicationTypes
{
    SystemMessage = 0,
    TaskMessage = 1,
    ProjectMessage = 2,
    JobMessage = 3,
    ResumeMessage = 4,
    ChatMessage = 5
}

public enum TaskType
{
    General = 0,
    Project = 1,
    Email = 2,
    Phone = 3,
    Meeting = 4,
    Interview = 5,
    Onboarding = 10,
    OnboardingProfile = 11,
    OnboardingProfileSettings = 12,
    OnboardingPrivacy = 13,

    Recruiter = 20,
    RecruiterPreferences = 21,
    JobSeeker = 30,
    JobSeekerPreferences = 31,
    Company = 40, //Update Company Profile; Add Contact Details; Add Job;
    CompanySettings = 41,
    Profile = 50, //Update Profile; Add Contact Details
    Job = 60,
    Resume = 70,
}

public enum ProductivityItemStatus
{
    Planning,
    ToDo,
    Open,
    InProgress,
    Closed,
    Descoped,
}

public enum JobApplicationStatus
{
    System, //System Recommendation or Match
    Saved, //Interested
    Applied,
    Longlist,
    Shortlist,
    Outbound,
    Screening,
    Submitted,
    Reviewing,
    Interviewing,
    Rejected,
    Closed,
    Hired
}

public enum AccountType
{
    Free,
    Monthly,
    Annual
}
public enum AccountUsageType
{
    Individual,
    Business,
    Employee
}

public enum JobOrigin
{
    JobSeeker,
    Employer,
    Crawler
}


public enum JobStatus
{
    Draft,
    Open,
    Closed,
    Hired
}
public enum DisplayMode { Edit, View }

public enum BonusType
{
    Dollar,
    Percentage
}
public enum BonusFrequency
{
    Annual,
    Quarterly,
    Monthly
}

public enum EquityType
{
    RSUs,
    Percentage
}

public enum FeeType
{
    Dollar,
    Flat,
    Hourly
}

public enum RejectionStatus
{
    [Description("Underqualified")]
    Underqualified,
    [Description("Overqualified")]
    Overqualified,
    [Description("Skills / Experience")]
    SkillsExperience,
    [Description("Personality Match")]
    PersonalityMatch,
    [Description("Not Interested in Job Responsibilities")]
    NotInterestedInJobResponsibilities,
    [Description("Not Interested in Company")]
    NotInterestedInCompany,
    [Description("Very happy in current role")]
    VeryHappyInCurrentRole,
    [Description("Accepted another offer")]
    AcceptedAnotherOffer,
    [Description("Outside Compensation Range")]
    OutsideCompensationRange,
    [Description("Not willing to work remote")]
    NotWillingToWorkRemote,
    [Description("Only interested in Remote")]
    OnlyInterestedInRemote,
    [Description("Commute Distance too far")]
    CommuteDistanceTooFar,
    [Description("Hybrid or Flex not available")]
    HybridNotAvailable,
}


public enum Industry
{
    [Description("Accounting")]
    Accounting = 0,
    [Description("Administration")]
    Administration = 1,
    [Description("Art Design")]
    ArtDesign = 2,
    [Description("Business Sales")]
    BusinessSales = 3,
    [Description("Construction")]
    Construction = 4,
    [Description("Education")]
    Education = 5,
    [Description("Engineering")]
    Engineering = 6,
    [Description("Food Production")]
    FoodProduction = 7,
    [Description("Healthcare")]
    Healthcare = 8,
    [Description("Human Resources (HR)")]
    HumanResources = 9,
    [Description("Information Technology (IT)")]
    IT = 10,
    [Description("Legal")]
    Legal = 11,
    [Description("Management")]
    Management = 12,
    [Description("Manufacturing")]
    Manufacturing = 13,
    [Description("Marketing / Public Relations (PR)")]
    MarketingPR = 14,
    [Description("Customer Service")]
    CustomerService = 15,
    [Description("Science")]
    Science = 16,
    [Description("Software Engineering")]
    SoftwareEngineering = 17,
    [Description("Transportation Logistics")]
    TransportationLogistics = 18,
    [Description("Hospitality")]
    Hospitality = 19,
    [Description("Government")]
    Government = 20,
    [Description("Finance")]
    Finance = 21,
    [Description("Corporate Training")]
    CorporateTraining = 22,
    [Description("Facilities")]
    Facilities = 23,
    [Description("Law Enforcement Security")]
    LawEnforcementSecurity = 24,
    [Description("Media")]
    Media = 25,
    [Description("Pharmaceuticals")]
    Pharmaceuticals = 26,
    [Description("Retail")]
    Retail = 27,
    [Description("Real Estate")]
    RealEstate = 28,
    [Description("Production")]
    Production = 29,
    [Description("Travel Tourism")]
    TravelTourism = 30,
}

public enum ExperienceLevel
{
    [Description("Entry")]
    Entry = 0,
    [Description("Junior")]
    Junior = 1,
    [Description("Mid-Career")]
    Mid = 2,
    [Description("Senior")]
    Senior = 3,
    [Description("Executive")]
    LeadExecutive = 4,
    [Description("None")]
    None = 5
}

public enum JobType
{
    [Description("Full-Time")]
    FullTime = 0,
    [Description("Part-Time")]
    PartTime = 1,
    [Description("Freelance")]
    Freelance = 2,
    [Description("Contractor")]
    Contractor = 3,
    [Description("Contract-To-Hire")]
    ContractToHire = 4,
    [Description("Temporary")]
    Temporary = 5,
    [Description("Internship")]
    Internship = 6
}

public enum WorkSetting
{
    [Description("Onsite")]
    Onsite = 0,
    [Description("Remote")]
    Remote = 1,
    [Description("Hybrid")]
    Hybrid = 2
}

public enum PaySchedule
{
    Hourly, Daily, Weekly, BiWeekly, Monthly, Quarterly, Yearly
}
public enum MetaType
{
    Hyperlink = 0,
    VideoMP4 = 1,
    VideoWebM = 2,
    Image = 5,
    YouTube = 10,
    HtmlContent = 15,
}

public enum ProfileStatus
{
    NoStatus,
    OpenToWork,
    Hiring,
}

public enum DataCollectionTypes
{
    Resume,
    ResumeEmbedded,
    UserProfile,
    UserProfileEmbedded,
    CompanyProfile,
    CompanyProfileEmbedded,
    CompanyJobPage,
    CompanyJobPageEmbedded,
    Job,
    JobEmbedded
}

public enum ResumeType
{
    ResumeBuilder = 0,
    JSONResumeFormat = 1,
    WordDoc = 2,
    Pdf = 3,
}

public enum ErrorCodes
{
    SystemError = 0,
    InValidUser = 1,
    InValidSecurityCode = 2,
    AccountUnconfirmed = 10,
    NoResume = 20,
    JobError = 30
}

public enum DistanceUnit
{
    [Description("Miles")]
    Miles = 0,
    [Description("Kilometers")]
    Kilometers = 1
}

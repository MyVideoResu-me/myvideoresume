using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;
using MyVideoResume.AI;
using MyVideoResume.Application.Job;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Data.Models.Resume;
using MyVideoResume.Web.Common;
using Radzen;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyVideoResume.Application.Resume;

public class MatchService : OpenAIPromptEngine
{
    private readonly JobService _jobService;
    private readonly ResumeService _resumeService;
    private readonly string jsonFormat = @"
    {
        ""SummaryRecommendations"": ""Summary Recommendations as Markdown"",
        ""Score"": ""float""
    }";

    private readonly string jsonResumeRecommendationFormat = @"
    {
        ""SummaryRecommendations"": ""Summary Recommendations as Markdown"",
        ""OldScore"": ""float"",
        ""NewScore"": ""float"",
        ""Resume"":
        {
          ""basics"": {
            ""name"": ""John Doe"",
            ""label"": ""Programmer"",
            ""image"": """",
            ""email"": ""john@gmail.com"",
            ""phone"": ""(912) 555-4321"",
            ""url"": ""https://johndoe.com"",
            ""summary"": ""A summary of John Doe…"",
            ""location"": {
              ""address"": ""2712 Broadway St"",
              ""postalCode"": ""CA 94115"",
              ""city"": ""San Francisco"",
              ""countryCode"": ""US"",
              ""region"": ""California""
            },
            ""profiles"": [{
              ""network"": ""Twitter"",
              ""username"": ""john"",
              ""url"": ""https://twitter.com/john""
            }]
          },
          ""work"": [{
            ""name"": ""Company"",
            ""position"": ""President"",
            ""url"": ""https://company.com"",
            ""startDate"": ""2013-01-01"",
            ""endDate"": ""2014-01-01"",
            ""summary"": ""Summary Description of all the highlights…"",
            ""highlights"": [
              ""Started the company"",
              ""Another accomplishment""
            ]
          }],
          ""volunteer"": [{
            ""organization"": ""Organization"",
            ""position"": ""Volunteer"",
            ""url"": ""https://organization.com/"",
            ""startDate"": ""2012-01-01"",
            ""endDate"": ""2013-01-01"",
            ""summary"": ""Description…"",
            ""highlights"": [
              ""Awarded 'Volunteer of the Month'""
            ]
          }],
          ""education"": [{
            ""institution"": ""University"",
            ""url"": ""https://institution.com/"",
            ""area"": ""Software Development"",
            ""studyType"": ""Bachelor"",
            ""startDate"": ""2011-01-01"",
            ""endDate"": ""2013-01-01"",
            ""score"": ""4.0"",
            ""courses"": [
              ""DB1101 - Basic SQL""
            ]
          }],
          ""awards"": [{
            ""title"": ""Award"",
            ""date"": ""2014-11-01"",
            ""awarder"": ""Company"",
            ""summary"": ""Description…""
          }],
          ""certificates"": [{
            ""name"": ""Certificate"",
            ""date"": ""2021-11-07"",
            ""issuer"": ""Company"",
            ""url"": ""https://certificate.com""
          }],
          ""publications"": [{
            ""name"": ""Publication"",
            ""publisher"": ""Company"",
            ""releaseDate"": ""2014-10-01"",
            ""url"": ""https://publication.com"",
            ""summary"": ""Description…""
          }],
          ""skills"": [{
            ""name"": ""Web Development"",
            ""level"": ""Master"",
            ""keywords"": [
              ""HTML"",
              ""CSS"",
              ""JavaScript""
            ]
          }],
          ""languages"": [{
            ""language"": ""English"",
            ""fluency"": ""Native speaker""
          }],
          ""interests"": [{
            ""name"": ""Wildlife"",
            ""keywords"": [
              ""Ferrets"",
              ""Unicorns""
            ]
          }],
          ""references"": [{
            ""name"": ""Jane Doe"",
            ""reference"": ""Reference…""
          }],
          ""projects"": [{
            ""name"": ""Project"",
            ""startDate"": ""2019-01-01"",
            ""endDate"": ""2021-01-01"",
            ""description"": ""Description..."",
            ""highlights"": [
              ""Won award at AIHacks 2016""
            ],
            ""url"": ""https://project.com/""
          }]
        }
    }";

    public MatchService(ILogger<MatchService> logger, JobService jobService, ResumeService resumeService, IConfiguration configuration) : base(logger, configuration)
    {
        _jobService = jobService;
        _resumeService = resumeService;
    }
    public async Task<ResponseResult<JobResumeMatchResponse>> MatchByJobContentResumeId(JobContentResumeIdMatchRequest request)
    {
        var r = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            //Get the Resume
            var resume = await _resumeService.GetResume(request.ResumeId);
            var result = await MatchByJobResumeContent(new JobResumeByContentMatchRequest() { JobContent = request.JobContent, ResumeContent = resume.ResumeSerialized });
            r = result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }

    public async Task<ResponseResult<JobResumeMatchResponse>> MatchByJobResumeId(JobResumeIdMatchRequest request)
    {
        var r = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            var resume = await _resumeService.GetResume(request.ResumeId);
            var job = await _jobService.GetJob(request.JobId);
            var result = await MatchByJobResumeContent(new JobResumeByContentMatchRequest() { JobContent = job.JobSerialized, ResumeContent = resume.ResumeSerialized });
            r = result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }

    public async Task<ResponseResult<JobResumeMatchResponse>> MatchByJobResumeContent(JobResumeByContentMatchRequest request)
    {
        var r = new ResponseResult<JobResumeMatchResponse>();
        try
        {
            var prompt = "You are an AI Assistant that helps people match their Resume to a Job Description. I need you to score how strong of a match from 0 to 100. 100 being a perfect match. Include a summary that provides examples of the relevant skills, work experience, projects of the resume that match the job description. If the score is below 80, include feedback, recommendations and next steps for the candidate to improve. Bold the headings. Return the score and summary in the given Json structure. Respond with no formatting for the JSON. The summary should be in Markdown.";
            var userResumeInput = $"Resume: {request.ResumeContent}";
            var userJobInput = $"Job Description: {request.JobContent}";
            var jsonFormatInput = $"JSON: {jsonFormat}";
            var result = await this.Process(prompt, new[] { userResumeInput, userJobInput, jsonFormatInput });
            var temp = JsonSerializer.Deserialize<JobResumeMatchResponse>(result.Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            r.Result = temp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }

    public async Task<ResponseResult<JobResumeBestResumeResponse>> BestResumeByJobResumeContent(JobResumeByContentMatchRequest request)
    {
        var r = new ResponseResult<JobResumeBestResumeResponse>();
        try
        {
            var prompt = "Given a person's Resume and Job Description, create a new resume that contains all the details of the job description. Keep all employment positions.  For positions in the resume that match the job description, update the resume's existing positions to include more content from the job description like keywords, required experience, and other required job duties. Extract skills and technical expertise from the Job Description and add to the resume profile summary and job positions. Update the resume's summary, job experience, skills, etc to include all your recommendations.  Return the new resume in the same JSON format. Make sure the resume is ATS-friendly. Update resume with all of your recommendations. Lastly, I need you to score the OLD resume and the new resume against the job description. The score is from 0 to 100. 100 being a perfect match. Try to get a perfect match for the new resume. Include a recommendation summary that provides details about the new resume. Return the result in the Json structure. Respond with no formatting for the JSON. The recommendations should be in Markdown.";
            var userResumeInput = $"Resume: {request.ResumeContent}";
            var userJobInput = $"Job Description: {request.JobContent}";
            var jsonFormatInput = $"JSON: {jsonResumeRecommendationFormat}";
            var result = await this.Process(prompt, new[] { userResumeInput, userJobInput, jsonFormatInput });
            var temp = JsonSerializer.Deserialize<JobResumeBestResumeResponse>(result.Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            r.Result = temp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            r.ErrorMessage = ex.Message;
        }
        return r;
    }


    
}

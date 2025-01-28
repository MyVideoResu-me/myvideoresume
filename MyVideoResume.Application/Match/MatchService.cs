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
}

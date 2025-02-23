using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Abstractions.Match;
using MyVideoResume.Application.Job;
using MyVideoResume.Application.Resume;
using MyVideoResume.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application;

public class Processor
{
    private readonly ILogger _logger;
    private readonly MatchService _matchService;
    private readonly JobService _jobService;
    private readonly ResumeService _resumeService;
    public Processor(ILogger<Processor> logger, MatchService matchService, JobService jobService, ResumeService resumeService)
    {
        _logger = logger;
        _jobService = jobService;
        _resumeService = resumeService;
        _matchService = matchService;
    }

    public async Task<ResponseResult<JobResumeBestResumeResponse>> ProcessChromeExtensionRequest(string userId, JobChromeRequest request)
    {
        var result = new ResponseResult<JobResumeBestResumeResponse>();
        _logger.LogInformation("Processing started");
        try
        {
            // Get the User's Resume
            var resume = await _resumeService.GetDefaultResume(userId);
            if (resume == null)
            {
                result.ErrorMessage = "No Resumes Configured";
                result.ErrorCode = ErrorCodes.NoResume;
                return result;
            }

            var job = await _jobService.ExtractJob(request.Html);
            if (!job.ErrorMessage.HasValue())
            {
                // Call Match Service to Generate Best Resume
                var bestResumeRequest = new JobResumeByContentMatchRequest { JobContent = job.Result.JobSerialized, ResumeContent = resume.ResumeSerialized };
                result = await _matchService.BestResumeByJobResumeContent(bestResumeRequest);
                //TODO FUTURE:
                // 1. Save the Job
                // 2. Add the Job to the User's Job List
                // 3. Create Tasks for the Person to Complete
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex.Message, ex);
        }

        // Do some processing
        _logger.LogInformation("Processing completed");

        return result;
    }

    public async Task<ResponseResult<JobResumeMatchResponse>> ProcessJobResumeMatch(string userId, JobChromeRequest request)
    {
        var result = new ResponseResult<JobResumeMatchResponse>();
        _logger.LogInformation("Processing started");
        try
        {
            //TODO Cache this... 
            // Get the User's Resume
            var resume = await _resumeService.GetDefaultResume(userId);
            if (resume == null)
            {
                result.ErrorMessage = "No Resumes Configured";
                result.ErrorCode = ErrorCodes.NoResume;
                return result;
            }

            //TODO Cache This: 
            var job = await _jobService.ExtractJob(request.Html);
            if (!job.ErrorMessage.HasValue())
            {
                // Call Match Service to Generate Best Resume
                var bestResumeRequest = new JobResumeByContentMatchRequest { JobContent = job.Result.JobSerialized, ResumeContent = resume.ResumeSerialized };
                result = await _matchService.MatchByJobResumeContent(bestResumeRequest);
                //TODO FUTURE:
                // 1. Save the Job
                // 2. Add the Job to the User's Job List
                // 3. Create Tasks for the Person to Complete
            }
            else
            {
                result.ErrorMessage = job.ErrorMessage;
                result.ErrorCode = ErrorCodes.JobError;
            }
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex.Message, ex);
        }

        // Do some processing
        _logger.LogInformation("Processing completed");

        return result;
    }

}

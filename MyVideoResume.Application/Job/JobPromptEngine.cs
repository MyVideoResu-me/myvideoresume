using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.AI;
using MyVideoResume.Data.Models.Jobs;
using MyVideoResume.Documents;
using System.Text.Json;

namespace MyVideoResume.Application.Job;

public interface IJobPromptEngine : IPromptEngine
{
    Task<ResponseResult<JobItemEntity>> ExtractJob(string jobDescription);
    Task<ResponseResult<JobItemEntity>> ExtractJobByUrl(string url);
}

public class JobPromptEngine : OpenAIPromptEngine, IJobPromptEngine
{
    private readonly DocumentProcessor _documentProcessor;
    private readonly string jsonFormat = @"
    {
        ""title"": ""Job Position Name"",
        ""description"": ""Description of the Job Posting"",
        ""ATSApplyUrl"": ""url of where to apply"",
        ""responsibilities"": [
            ""key responsibility""
        ],
        ""requirements"": [
            ""key responsibility""
        ],
    }";

    public JobPromptEngine(ILogger<JobPromptEngine> logger, IConfiguration configuration, DocumentProcessor processor) : base(logger, configuration)
    {
        _documentProcessor = processor;
    }

    public async Task<ResponseResult<JobItemEntity>> ExtractJob(string jobDescription)
    {
        var prompt = @"Given the job description parse it into JSON format. Do NOT summarize the content of the job description. Respond with no formatting.";

        var result = new ResponseResult<JobItemEntity>();
        try
        {
            var userInput = $"JSON: {jsonFormat}";
            var userJobInput = $"Job Description: {jobDescription}";
            var conversion = await this.Process(prompt, new[] { userInput, userJobInput });
            var temp = JsonSerializer.Deserialize<JobItemEntity>(conversion.Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            temp.JobSerialized = conversion.Result;
            result = new ResponseResult<JobItemEntity>() { Result = temp };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;

    }


    public async Task<ResponseResult<JobItemEntity>> ExtractJobByUrl(string url)
    {
        var prompt = @"Navigate to the URL and extract the job description into JSON format. Do NOT summarize the content of the job description. Respond with no formatting.";

        var result = new ResponseResult<JobItemEntity>();
        try
        {
            var userInput = $"JSON: {jsonFormat}";
            var userJobInput = $"Url: {url}";
            var conversion = await this.Process(prompt, new[] { userInput, userJobInput });
            var temp = JsonSerializer.Deserialize<JobItemEntity>(conversion.Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            temp.JobSerialized = conversion.Result;
            result = new ResponseResult<JobItemEntity>() { Result = temp };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
        }
        return result;

    }
}

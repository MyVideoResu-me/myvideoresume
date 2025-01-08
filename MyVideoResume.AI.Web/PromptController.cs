using Microsoft.AspNetCore.Mvc;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Job;
using MyVideoResume.Documents;

namespace MyVideoResume.AI;

[Route("[controller]")]
public class PromptController : Controller
{
    private readonly IPromptEngine _engine;
    private readonly DocumentProcessor _documentProcessor;
    public PromptController(IPromptEngine engine, DocumentProcessor processor)
    {
        // Get the ML Model Engine injected, for scoring
        _engine = engine;
        _documentProcessor = processor;
    }

    [HttpPost]
    [Route("summarize")]
    public async Task<ActionResult<ResponseResult>> SummarizeContent([FromBody] string content)
    {
        var prompt = "You are an AI Assistant that helps people summarize their resume.";
        var result = await _engine.Process(prompt, content);
        return result;
    }

    [HttpPost]
    [Route("match")]
    public async Task<ActionResult<ResponseResult>> MatchComparison([FromBody] MatchRequest request)
    {
        var prompt = "You are an AI Assistant that helps people match their Document to a Comparison Description.";
        var userInput = $"Document: {request.Reference}";
        var userJobInput = $"Comparison Description: {request.Comparison}";
        var result = await _engine.Process(prompt, new[] { userInput, userJobInput });
        return result;
    }

    [HttpPost]
    [Route("parse")]
    public async Task<ActionResult<ResponseResult>> ParseToJson(IFormFile file)
    {
        var prompt = @"you are a file parser assistant. I need you to parse the file into the following JSON format: 
        {
            //TODO provide Format
        }
        ";

        var result = new ResponseResult();
        try
        {
            if (file != null)
            {
                var content = _documentProcessor.PdfToString(file.OpenReadStream());
                var conversion = await _engine.Process(prompt, content);
                result = conversion;
            }
        }
        catch (Exception ex)
        {
            result.Result = ex.Message;
        }
        return result;

    }
}

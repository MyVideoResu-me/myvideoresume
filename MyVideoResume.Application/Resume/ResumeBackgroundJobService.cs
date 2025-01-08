using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MyVideoResume.Data;
using MyVideoResume.Web.Common;
using Radzen;
using System.Net.Http.Json;

namespace MyVideoResume.Application.Resume;

public class ResumeBackgroundJobService
{
    private readonly ILogger<ResumeBackgroundJobService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUri;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ResumeBackgroundJobService(ILogger<ResumeBackgroundJobService> logger, IServiceScopeFactory serviceScopeFactory, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _baseUri = configuration.GetValue<string>(Constants.BaseUriConfigurationProperty);
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(Constants.HttpClientFactory);
    }

    public async Task ProcessSemanticScore()
    {
        try
        {
            var uri = new Uri($"{_baseUri}{Paths.AI_API_Sentiment}");
            //Get all the Resumes that don't have a Semantic Score...
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
            headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
            //eventually set headers coming from other sources (e.g. consuming a queue) 
            headerPropagationValues.Headers.Add("User-Agent", "background-service");
            Console.WriteLine("Initialized Header Propagation Values");
            var _dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var items = _dataContext.ResumeInformation.Where(x => x.SentimentScore.HasValue == false && x.DeletedDateTime == null);
            Console.WriteLine($"Connected to Database. # of Resumes needing Semantic Value: {items.Count()}");
            foreach (var item in items)
            {
                var response = await _httpClient.PostAsJsonAsync<string>(uri, item.ResumeSerialized);
                var value = await response.ReadAsync<float>();
                item.SentimentScore = value;
                item.UpdateDateTime = DateTime.UtcNow;
                Console.WriteLine($"Resume ID: {item.Id} - Semantic Score: {value}");
            }
            _dataContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _logger.LogError(ex.Message, ex);
        }
    }
}

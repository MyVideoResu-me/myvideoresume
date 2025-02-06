using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyVideoResume.Abstractions.DataCollection;
using System.Security.Claims;
using UAParser;  // Import the UAParser namespace
using MyVideoResume.Data.Models.DataCollection;
using Microsoft.Extensions.Configuration;
using MyVideoResume.Abstractions.Core;
using System.Security.Policy;
using Splitio.Constants;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Application.DataCollection;


public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRequestLogger _requestLogger;
    private readonly Parser _uaParser; // Instance of UAParser, created once per request
    private readonly IConfiguration _configuration;

    public RequestLoggingMiddleware(RequestDelegate next, IRequestLogger requestLogger, Parser uaParser, IConfiguration configuration)
    {
        _next = next;
        _requestLogger = requestLogger;
        _uaParser = uaParser; // Initialize the UAParser once per request
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var dataCollectionEnabled = _configuration.GetValue<bool>("DataCollection");

            if (dataCollectionEnabled)
            {
                var requestPath = context.Request.Path.ToString();

                if (!requestPath.Contains("/api/"))
                {
                    DataCollectionTypes? dataCollectionType = null;
                    //TODO: add CompanyProfile, CompanyJobPage, Embedded pages.
                    if (requestPath.Contains("/resumes/", StringComparison.OrdinalIgnoreCase) || requestPath.Contains("/resume/", StringComparison.OrdinalIgnoreCase))
                    {
                        dataCollectionType = DataCollectionTypes.Resume;
                    }
                    if (requestPath.Contains("/jobs/", StringComparison.OrdinalIgnoreCase) || requestPath.Contains("/job/", StringComparison.OrdinalIgnoreCase))
                    {
                        dataCollectionType = DataCollectionTypes.Job;
                    }
                    if (requestPath.Contains("/people/profile/", StringComparison.OrdinalIgnoreCase) || requestPath.Contains("/people/profiles/", StringComparison.OrdinalIgnoreCase))
                    {
                        dataCollectionType = DataCollectionTypes.UserProfile;
                    }

                    //only capture data for those specific endpoints for Data Collection
                    if (dataCollectionType.HasValue)
                    {
                        string? extractValue = ExtractId(requestPath, ["/resumes/", "/resume/", "/jobs/", "/job/", "/people/profile/", "/people/profiles/"]);

                        // Extract and parse the User-Agent string using UAParser
                        var userAgentString = context.Request.Headers["User-Agent"].ToString();
                        var uaResult = _uaParser.Parse(userAgentString);  // Parse the User-Agent

                        // Capture referrer if available
                        var referrer = context.Request.Headers["Referer"].ToString();

                        // Capture request details
                        var requestLog = new RequestLogEntity
                        {
                            Status = BatchProcessStatus.NotStarted,
                            DataCollectionType = dataCollectionType,
                            DataCollectionId = extractValue,
                            Url = requestPath,
                            Method = context.Request.Method,
                            UserAgent = userAgentString,  // Original User-Agent string
                            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                            CreationDateTime = DateTime.UtcNow,
                            UserId = GetUserIdFromClaims(context),  // Extract User ID
                            Browser = uaResult.UserAgent.Family,   // Browser name (e.g., Chrome, Firefox)
                            BrowserVersion = uaResult.UserAgent.ToString(),  // Full browser version string (e.g., Chrome/91.0.4472)
                            OS = uaResult.OS.Family,              // Operating system name (e.g., Windows, macOS)
                            OSVersion = uaResult.OS.ToString(),   // Full operating system version (e.g., Windows 10)
                            Device = uaResult.Device.Family,      // Device type (e.g., Desktop, Mobile, Tablet)
                            Referrer = referrer                  // Capture the referrer URL
                        };

                        // Log the request asynchronously in the background using _ (discard => fire & forget)
                        _ = LogRequestInBackground(requestLog);

                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error locally or use another logging system to track errors
            Console.WriteLine($"Error creating request log: {ex.Message}");
        }

        // Continue processing the request
        await _next(context);
    }

    private static string ExtractId(string requestPath, string[] prefixes)
    {
        var extractedValue = string.Empty;

        foreach (var prefix in prefixes)
        {
            int startIndex = requestPath.IndexOf(prefix);

            if (startIndex != -1)
            {
                // Extract the substring after "/resumes/"
                extractedValue = requestPath.Substring(startIndex + prefix.Length);
            }
        }
        return extractedValue;
    }

    // Use a background task to log the request asynchronously
    private async Task LogRequestInBackground(RequestLogEntity requestLog)
    {
        try
        {
            await _requestLogger.LogRequestAsync(requestLog);
        }
        catch (Exception ex)
        {
            // Optionally handle errors during logging
            Console.WriteLine($"Error logging request to database: {ex.Message}");
        }
    }

    // Method to get the logged-in user's ID from claims
    private string GetUserIdFromClaims(HttpContext context)
    {
        // Extract user ID from ClaimsPrincipal (assuming the user ID is stored in "NameIdentifier" claim)
        return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

﻿using System;
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
using MyVideoResume.Extensions;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using System.Reflection.Metadata;
using MyVideoResume.Web.Common;
using Microsoft.Extensions.Logging;

namespace MyVideoResume.Application.DataCollection;


public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRequestLogger _requestLogger;
    private readonly Parser _uaParser; // Instance of UAParser, created once per request
    private readonly IConfiguration _configuration;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, IRequestLogger requestLogger, Parser uaParser, IConfiguration configuration, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _requestLogger = requestLogger;
        _uaParser = uaParser; // Initialize the UAParser once per request
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var dataCollectionEnabled = _configuration.GetValue<bool>("DataCollection");

            if (dataCollectionEnabled)
            {
                var request = context.Request;
                var requestPath = request.Url();

                if (!requestPath.Contains("/api/") && !requestPath.Contains("/swagger") && !requestPath.Contains("/openapi"))
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
                        Uri uri = new Uri(requestPath);
                        NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query); //(get all the referrer information)
                        var campaignId = queryParams[Constants.CampaignId];
                        var referrerUserId = queryParams[Constants.ReferrerUserId];

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
                            CampaignId = campaignId, //Was this part of a Campaign?
                            ReferrerUserId = referrerUserId, //Is this a Share via email by a User?
                            UserId = GetUserIdFromClaims(context),  // Extract User ID
                            Browser = uaResult.UA.Family,   // Browser name (e.g., Chrome, Firefox)
                            BrowserVersion = uaResult.UA.ToString(),  // Full browser version string (e.g., Chrome/91.0.4472)
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

            // Continue processing the request
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the error locally or use another logging system to track errors
            Console.WriteLine($"Error creating request log: {ex.Message}");
            _logger.LogError(ex.Message, ex);
        }
    }

    private static string ExtractId(string requestPath, string[] prefixes)
    {
        var extractedValue = string.Empty;

        string slugOrGuidPattern = @"/(?:resumes?|job|jobs?|person|profile)/([^/?]+)";  // Match slug or GUID before query string
        Match match = Regex.Match(requestPath, slugOrGuidPattern);

        extractedValue = match.Success ? match.Groups[1].Value : string.Empty;  // Extract GUID if found

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

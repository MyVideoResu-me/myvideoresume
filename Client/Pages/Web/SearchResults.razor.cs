using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Radzen;
using Radzen.Blazor;
using System.ComponentModel;
using System.Net.Http.Json;
using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Client.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace MyVideoResume.Client.Pages.Web;

public partial class SearchResults
{
    [Inject]
    protected ResumeWebService ResumeService { get; set; }
    
    [Inject]
    protected SecurityWebService SecurityService { get; set; }
    
    [SupplyParameterFromQuery(Name = "Query")]
    private string? Query { get; set; }

    [SupplyParameterFromQuery(Name = "Filter")]
    private string? Filter { get; set; }
    
    [SupplyParameterFromQuery(Name = "Education")]
    private string? Education { get; set; }
    
    [SupplyParameterFromQuery(Name = "Experience")]
    private string? Experience { get; set; }
    
    [SupplyParameterFromQuery(Name = "Radius")]
    private string? Radius { get; set; }
    
    private List<ResumeInformationSummaryDTO> searchResults = new List<ResumeInformationSummaryDTO>();
    private bool isLoading = false;
    private double? userLatitude;
    private double? userLongitude;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        if (SecurityService.IsAuthenticated)
        {
            var userProfile = await SecurityService.GetUserProfile();
            if (userProfile != null && userProfile.Latitude.HasValue && userProfile.Longitude.HasValue)
            {
                userLatitude = userProfile.Latitude;
                userLongitude = userProfile.Longitude;
            }
        }
        
        await PerformSearch();
    }
    
    private async Task PerformSearch()
    {
        isLoading = true;
        
        var searchRequest = new ResumeSearchRequestDTO
        {
            TextQuery = Query,
            Education = Education,
            Experience = Experience,
            Latitude = userLatitude,
            Longitude = userLongitude
        };
        
        if (!string.IsNullOrEmpty(Radius) && double.TryParse(Radius, out double radiusValue))
        {
            searchRequest.RadiusMiles = radiusValue;
        }
        
        searchResults = await ResumeService.SearchResumes(searchRequest);
        
        isLoading = false;
    }
    
    protected override async Task OnParametersSetAsync()
    {
        await PerformSearch();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JSRuntime.InvokeVoidAsync("BlazorSetTitle", Query, Filter);
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyVideoResume.Client.Services;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using System.ComponentModel;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Abstractions.Account.Profiles;
using MyVideoResume.Client.Pages.Shared.Security;
using MyVideoResume.Client.Pages.Shared;
using Microsoft.AspNetCore.Http;

namespace MyVideoResume.Client.Pages;

public class BaseComponent : LayoutComponentBase
{
    [Inject] protected FeatureFlagClientService FeatureFlagService { get; set; }

    [Inject] protected MenuService MenuService { get; set; }

    [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject] protected HttpClient Http { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected IJSRuntime JSRuntime { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }

    [Inject] protected SecurityWebService Security { get; set; }

    public async Task<dynamic> ShowUnAuthorizedNoClose(string returnPath)
    {
        var options = new DialogOptions() { CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, ShowClose = false };
        var result = await ShowUnAuthorizedOptions(returnPath, options);
        return result;
    }
    public async Task<dynamic> ShowUnAuthorized(string returnPath)
    {
        var result = await ShowUnAuthorizedOptions(returnPath, null);
        return result;
    }

    public async Task<dynamic> ShowShareOptions(string urlToShare, string hashtags, string title)
    {
        var param = new Dictionary<string, object>();
        param.Add("UrlToShare", urlToShare);
        param.Add("Hashtags", hashtags);
        param.Add("Title", title);
        var result = await DialogService.OpenAsync<ShareOptionsComponent>("Share Options", param);
        return result;
    }

    private async Task<dynamic> ShowUnAuthorizedOptions(string returnPath, DialogOptions options)
    {
        var param = new Dictionary<string, object>();
        param.Add("Path", returnPath);
        var result = await DialogService.OpenAsync<UnAuthorizedComponent>("Sign In / Create Account", param, options);
        return result;
    }

    public async Task<dynamic> ShowRoleSelector(UserProfileDTO profile)
    {
        var param = new Dictionary<string, object>();
        param.Add("Profile", profile);
        var result = await DialogService.OpenAsync<WelcomeRoleSelectorComponent>("Welcome to MyVideoResu.ME", param, new DialogOptions() { CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, ShowClose = false });
        return result;
    }

    public void ShowSuccessNotification(string title, string message)
    {
        ShowNotification(title, message, NotificationSeverity.Success);
    }

    public void ShowErrorNotification(string title, string message)
    {

        ShowNotification(title, message, NotificationSeverity.Error);
    }

    private void ShowNotification(string title, string message, NotificationSeverity severity)
    {
        NotificationService.Notify(new NotificationMessage { Severity = severity, Summary = title, Detail = message, Duration = 4000 });
    }

    public void ShowTooltip(ElementReference elementReference, string content) => TooltipService.Open(elementReference, content, new TooltipOptions() { Position = TooltipPosition.Top });

    public void NavigateToLogin(string redirectPath)
    {
        NavigationManager.NavigateTo(NavigateToLoginPath(redirectPath));
    }

    public string NavigateToLoginPath(string redirectPath) { return $"login?redirectUrl={redirectPath}"; }

    public void NavigateTo(string path)
    {
        NavigationManager.NavigateTo(path);
    }

    public void NavigateTo(string path, string parameter)
    {
        NavigationManager.NavigateTo($"{path}/{parameter}");
    }

    public Type ResolveComponent(string componentName, string namespacevalue)
    {
        return string.IsNullOrEmpty(componentName) ? null
            : Type.GetType($"{namespacevalue}.{componentName}");
    }

    protected async Task DownloadHtmlFile(string content, string fileName)
    {
        await DownloadFile(content, fileName, "html");
    }

    protected async Task DownloadJsonFile(string content, string filename)
    {
        await DownloadFile(content, filename, "json");
    }

    private async Task DownloadFile(string content, string filename, string fileextension)
    {
        await JSRuntime.InvokeVoidAsync("saveTextAsFile", content, $"{filename}-{DateTime.Now.ToString("yyyy-MM-dd")}.{fileextension}");
    }
}

using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace MyVideoResume.Client.Layout;

public partial class MainLayout
{
    [Inject] ThemeService ThemeService { get; set; }

    const string logodark = "images/logo-login.png";
    const string logolight = "images/logo-login-dark.png";
    public string Logo { get; set; } = logolight;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        OnThemeChanged();
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    void OnThemeChanged()
    {
        switch (ThemeService.Theme)
        {
            case "standard-dark":
                Logo = logolight;
                break;
            case "standard":
                Logo = logodark;
                break;
        }
    }
    protected void ProfileMenuClick(RadzenProfileMenuItem args)
    {
        if (args.Value == "Logout")
        {
            Security.Logout();
        }
    }

    public bool ShowLogin
    {
        get
        {
            return !Security.IsAuthenticated();
        }
    }
}

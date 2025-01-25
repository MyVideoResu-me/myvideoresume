using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Radzen;
using MyVideoResume.Client.Shared.Security.Recaptcha;
using Radzen.Blazor;
using MyVideoResume.Web.Common;

namespace MyVideoResume.Client.Pages.Web;

public partial class RegisterApplicationUser
{
    protected Data.Models.ApplicationUser user;
    protected bool IsBusy { get; set; }
    protected bool errorVisible;
    protected string error;

    [Inject] IConfiguration Configuration { get; set; }
    [Inject] ILogger<RegisterApplicationUser> Logger { get; set; }


    protected bool isCaptchaValid;
    string token = "";
    RecaptchaResponse? response;

    protected override async void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (Configuration.GetValue<bool>("Security:IsCaptchaEnabled"))
            {
                token = await JSRuntime.InvokeAsync<string>("runCaptcha");
                StateHasChanged();
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        user = new Data.Models.ApplicationUser();
    }

    protected async Task FormSubmit(Data.Models.ApplicationUser user)
    {
        IsBusy = true;
        errorVisible = false;
        error = string.Empty;

        try
        {
            if (Configuration.GetValue<bool>("Security:IsCaptchaEnabled"))
            {
                //if this is empty the form was already ran...
                if(!token.HasValue())
                    token = await JSRuntime.InvokeAsync<string>("runCaptcha");

                response = await Security.VerifyRecaptcha(token);
                isCaptchaValid = response.success;
                if (isCaptchaValid)
                    token = string.Empty;
            }
            else
                isCaptchaValid = true;


            if (isCaptchaValid)
            {
                if (user.Password != user.ConfirmPassword) {
                    errorVisible = true;
                    error = "Passwords Must Match";
                }
                else
                {
                    await Security.Register(user.Email, user.Password);
                    DialogService.Close(true);
                }
            }
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }

        IsBusy = false;
    }

    protected async Task OnInvalidSubmit() { }

    protected async Task CancelClick()
    {
        IsBusy = false;

        DialogService.Close(false);
    }
}
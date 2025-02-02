using MyVideoResume.Web.Common;
using MyVideoResume.Abstractions.Account.Profiles;

namespace MyVideoResume.Client.Pages.App;


public class AppBaseComponent : BaseComponent
{
    public UserProfileDTO Profile { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
        {
            var response = await Security.GetUserProfile();
            if (response.ErrorMessage.HasValue())
                ShowErrorNotification("Error", "Error Loading Profile");
            else
            {
                Profile = response.Result;
                StateHasChanged();
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Profile != null && (Profile.IsRoleSelected == null || Profile.IsRoleSelected == false))
            {
                //TODO: Show the Welcome Message and Select a Role. Clean UP the Resume.
                await ShowRoleSelector(Profile);
            }
        }
    }

}
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using MyVideoResume.Client.Pages.App.Admin;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Data.Models;
using MyVideoResume.Abstractions.Account;
using Microsoft.JSInterop; // Added for IJSRuntime
using System.Net.Http; // Added for HttpClient
using System.Threading.Tasks; // Added for Task

namespace MyVideoResume.Client.Pages.App.Account;

public partial class AccountSettings
{
    [Inject] protected IJSRuntime JSRuntime { get; set; } // Injected IJSRuntime
    [Inject] protected HttpClient Http { get; set; } // Injected HttpClient

    protected int tabSelected = 0;
    protected string oldPassword = "";
    protected string newPassword = "";
    protected string confirmPassword = "";
    protected bool value;
    protected bool isBusy;
    protected string error;
    protected bool errorVisible;
    protected bool successVisible;
    protected List<UserCompanyRoleAssociationEntity> users;
    protected RadzenDataGrid<ApplicationUser> grid0;
    public AccountSettingsDTO Settings { get; set; } = new AccountSettingsDTO();
    SortedList<string, string> DisplayPrivacyOptions = DisplayPrivacy.ToPublic.ToSortedList();
    public string DisplayPrivacyOptionSelected { get; set; } = DisplayPrivacy.ToPublic.ToString();
    public string DisplayPrivacyOptionContactDetailsSelected { get; set; } = DisplayPrivacy.ToPublic.ToString();


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Action", out var action))
        {
            switch (action)
            {
                case "security":
                    tabSelected = 2;
                    break;
                default:
                    tabSelected = 0;
                    break;
            }
        }

        var result = await Account.AccountSettingsRead();
        if (!result.ErrorMessage.HasValue())
        {
            Settings = result.Result;

            DisplayPrivacyOptionSelected = Settings.Privacy_ShowProfile.Value.ToString();
            DisplayPrivacyOptionContactDetailsSelected = Settings.Privacy_ShowProfileContactDetails.Value.ToString();

            //if the RoleSelect is null then they navigated directly here...
            //So we need to set a default and allow them to change it.
            if (Settings.RoleSelected == null)
            {
                var roleSelected = MyVideoResumeRoles.Recruiter;
                //Account.UserProfileUpdateRole(Settings, roleSelected); //Default to Job Seeker.
                Settings.RoleSelected = roleSelected;
            }
            else if (Settings.RoleSelected.Value == MyVideoResumeRoles.Recruiter)
            {
                value = false;
            }
            else
                value = true;
        }
    }

    protected async Task OnChange(int index)
    {
        if (index == 1)
        {
            await GetUsers();
        }
    }

    private async Task GetUsers()
    {
        var result = await Account.AccountUsers();
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Error", result.ErrorMessage);
        }
        else
        {
            users = result.Result;
        }
    }


    protected async Task AddClick()
    {
        await DialogService.OpenAsync<AddApplicationUser>("Add Application User");

        await GetUsers();
    }

    protected async Task ViewProfileDetails(UserCompanyRoleAssociationEntity user)
    {
        await DialogService.OpenAsync<ViewProfileDetails>("View User", new Dictionary<string, object> { { "Id", user.Id } });
    }


    protected async Task RowSelect(UserCompanyRoleAssociationEntity user)
    {
        await DialogService.OpenAsync<EditApplicationUser>("Edit Application User", new Dictionary<string, object> { { "Id", user.Id } });

        await GetUsers();
    }

    protected async Task DeleteClick(UserCompanyRoleAssociationEntity user)
    {
        try
        {
            if (await DialogService.Confirm("Are you sure you want to delete this user?") == true)
            {
                await Security.DeleteApplicationUser($"{user.Id}");

                await GetUsers();
            }
        }
        catch (Exception ex)
        {
            errorVisible = true;
            error = ex.Message;
        }
    }

    private async Task HandleValidSubmit()
    {
        // Save the updated user profile data to the server
        await Http.PutAsJsonAsync("api/userprofile", Settings);
    }
    protected async Task SaveSecurity()
    {
        isBusy = true;
        try
        {
            await Security.PasswordChange(oldPassword, newPassword);
            ShowSuccessNotification("Success", "Password Updated");
            successVisible = true;
        }
        catch (Exception ex)
        {
            ShowErrorNotification("Error", ex.Message);
        }
        isBusy = false;
    }

    protected async Task SaveUserProfile()
    {
        isBusy = true;
        try
        {
            var roleSelected = MyVideoResumeRoles.JobSeeker;
            if (!value)
                roleSelected = MyVideoResumeRoles.Recruiter;

            Settings.RoleSelected = roleSelected;

            Settings.Privacy_ShowProfile = Enum.Parse<DisplayPrivacy>(DisplayPrivacyOptionSelected);
            Settings.Privacy_ShowProfileContactDetails = Enum.Parse<DisplayPrivacy>(DisplayPrivacyOptionContactDetailsSelected);

            var result = await Account.UserProfileUpdate(Settings.CreateUserProfile());
            if (result.ErrorMessage.HasValue())
            {
                ShowErrorNotification("Error", "Error Saving");
            }
            else
            {
                ShowSuccessNotification("Success", "Saved Account Settings");
            }
        }
        catch (Exception ex)
        {
            ShowErrorNotification("Error", ex.Message);
        }
        isBusy = false;
    }

    protected async Task SavePreferences()
    {
        isBusy = true;
        try
        {
            await SaveUserProfile();
        }
        catch (Exception ex)
        {

        }
        isBusy = false;
    }

    // Method to get the client secret for a Stripe SetupIntent from the backend
    private async Task<string> GetSetupIntentClientSecret()
    {
        try
        {
            Console.WriteLine("GetSetupIntentClientSecret: Attempting to fetch client secret from API.");
            var response = await Http.PostAsync("api/payment/setup-intent", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SetupIntentResponse>();
                if (result != null && !string.IsNullOrWhiteSpace(result.ClientSecret))
                {
                    Console.WriteLine("GetSetupIntentClientSecret: Successfully fetched client secret.");
                    return result.ClientSecret;
                }
                else
                {
                    var errorMsg = "Failed to parse client secret from server response.";
                    ShowErrorNotification("Payment Setup Error", errorMsg);
                    Console.WriteLine($"GetSetupIntentClientSecret: {errorMsg}");
                    return null;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMsg = $"Failed to get setup intent from server. Status: {response.StatusCode}.";
                ShowErrorNotification("Payment Setup Error", "Could not initialize payment form. Please try again or contact support if the issue persists.");
                Console.WriteLine($"GetSetupIntentClientSecret: {errorMsg} Details: {errorContent}");
                return null;
            }
        }
        catch (HttpRequestException httpEx)
        {
            var errorMsg = "A network error occurred while initializing the payment form.";
            ShowErrorNotification("Network Error", $"{errorMsg} Please check your connection and try again.");
            Console.WriteLine($"GetSetupIntentClientSecret: HttpRequestException - {httpEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            var errorMsg = "An unexpected error occurred while initializing the payment form.";
            ShowErrorNotification("Payment Setup Error", $"{errorMsg} Please try again.");
            Console.WriteLine($"GetSetupIntentClientSecret: Exception - {ex.Message}");
            return null;
        }
    }
    private class SetupIntentResponse { public string ClientSecret { get; set; } }


    // Method to save the PaymentMethod ID to the backend
    private async Task SavePaymentMethod(string paymentMethodId)
    {
        // isBusy is typically set by the caller (HandleStripeSuccess) which wraps this.
        // However, if called directly, ensure isBusy is managed.
        // For this flow, HandleStripeSuccess manages isBusy.
        try
        {
            Console.WriteLine($"SavePaymentMethod: Attempting to save PaymentMethod ID: {paymentMethodId}");
            var requestData = new { paymentMethodId = paymentMethodId };
            var response = await Http.PostAsJsonAsync("api/payment/save-payment-method", requestData);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"SavePaymentMethod: Successfully saved PaymentMethod ID: {paymentMethodId}");
                ShowSuccessNotification("Success", "Your payment method has been saved successfully!");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorMsg = "Could not save your payment method at this time.";
                ShowErrorNotification("Save Payment Method Failed", $"{errorMsg} Please try again or contact support.");
                Console.WriteLine($"SavePaymentMethod: API call failed. Status: {response.StatusCode}, Content: {errorContent}");
            }
        }
        catch (HttpRequestException httpEx)
        {
            var errorMsg = "A network error occurred while saving your payment method.";
            ShowErrorNotification("Network Error", $"{errorMsg} Please check your connection and try again.");
            Console.WriteLine($"SavePaymentMethod: HttpRequestException - {httpEx.Message}");
        }
        catch (Exception ex)
        {
            var errorMsg = "An unexpected error occurred while saving your payment method.";
            ShowErrorNotification("Save Payment Method Error", $"{errorMsg} Please try again.");
            Console.WriteLine($"SavePaymentMethod: Exception - {ex.Message}");
        }
    }

    // This method will be called by the "Save Card" button
    protected async Task SaveCard()
    {
        isBusy = true;
        try
        {
            Console.WriteLine("SaveCard: Attempting to confirm card setup with Stripe.js.");
            await JSRuntime.InvokeVoidAsync("stripeInterop.confirmCardSetupOnStripe", DotNetObjectReference.Create(this));
            // isBusy will be set to false in HandleStripeSuccess or HandleStripeError
        }
        catch (JSException jsEx)
        {
            var errorMsg = "An error occurred while preparing your card details.";
            ShowErrorNotification("Payment Error", $"{errorMsg} Please try again. Details: {jsEx.Message}");
            Console.WriteLine($"SaveCard: JSException - {jsEx.Message}");
            isBusy = false;
        }
        catch (Exception ex)
        {
            var errorMsg = "An unexpected error occurred while attempting to save your card.";
            ShowErrorNotification("Payment Error", $"{errorMsg} Please try again. Details: {ex.Message}");
            Console.WriteLine($"SaveCard: Exception - {ex.Message}");
            isBusy = false;
        }
    }


    [JSInvokable]
    public async Task HandleStripeSuccess(string paymentMethodId)
    {
        try
        {
            Console.WriteLine($"HandleStripeSuccess: Card details processed by Stripe. PaymentMethod ID: {paymentMethodId}. Now saving to backend.");
            // Show intermediate success from Stripe if desired, though final confirmation from backend is better.
            // ShowSuccessNotification("Stripe Success", "Card details verified. Saving payment method...");
            await SavePaymentMethod(paymentMethodId);
        }
        catch (Exception ex) // Catch any unexpected errors from SavePaymentMethod if it doesn't handle them all
        {
            ShowErrorNotification("Save Error", "An unexpected error occurred after processing the card. Please contact support.");
            Console.WriteLine($"HandleStripeSuccess: Exception during SavePaymentMethod call - {ex.Message}");
        }
        finally
        {
            isBusy = false; // Ensure isBusy is reset here after all operations
        }
    }

    [JSInvokable]
    public async Task HandleStripeError(string stripeErrorMessage)
    {
        Console.WriteLine($"HandleStripeError: Stripe.js reported an error: {stripeErrorMessage}");
        // Provide a user-friendly message. Stripe's messages are often good, but we can generalize.
        var userFriendlyMessage = "An error occurred while processing your card. Please check your card details and try again.";
        // Log the detailed error from Stripe for debugging.
        ShowErrorNotification("Card Processing Error", userFriendlyMessage);
        Console.WriteLine($"HandleStripeError: Displayed user-friendly message. Original Stripe error: {stripeErrorMessage}");
        isBusy = false;
        await Task.CompletedTask;
    }


    // This method will call JavaScript to initialize Stripe Elements
    private async Task InitializeStripe()
    {
        try
        {
            Console.WriteLine("InitializeStripe: Attempting to initialize Stripe Elements.");
            string publishableKey = "pk_test_YOUR_STRIPE_PUBLISHABLE_KEY_HERE"; // TODO: Fetch from configuration

            string clientSecret = await GetSetupIntentClientSecret();
            if (string.IsNullOrEmpty(clientSecret))
            {
                // GetSetupIntentClientSecret already shows an error notification and logs.
                Console.WriteLine("InitializeStripe: Client secret is null or empty. Aborting initialization.");
                // Optionally, disable the payment form or show a persistent error message on the UI here.
                return;
            }
            
            Console.WriteLine("InitializeStripe: Client secret obtained. Calling JS interop to initialize Stripe Elements.");
            await JSRuntime.InvokeVoidAsync("stripeInterop.initializeStripeElements",
                publishableKey,
                "card-element",
                "card-errors",
                "submit-button",
                clientSecret,
                DotNetObjectReference.Create(this));

            Console.WriteLine("InitializeStripe: stripeInterop.initializeStripeElements call completed.");
        }
        catch (JSException jsEx)
        {
            var errorMsg = "Failed to initialize the payment form due to a script error.";
            ShowErrorNotification("Initialization Error", $"{errorMsg} Please try refreshing the page.");
            Console.WriteLine($"InitializeStripe: JSException - {jsEx.Message}");
        }
        catch (Exception ex) // Catch any other unexpected errors during initialization
        {
            var errorMsg = "An unexpected error occurred while setting up the payment form.";
            ShowErrorNotification("Initialization Error", $"{errorMsg} Please try again.");
            Console.WriteLine($"InitializeStripe: Exception - {ex.Message}");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        // We need to initialize Stripe only when the "Billing / Plan" tab AND its "Payment Methods" sub-tab are active.
        // The current `tabSelected` refers to the main tabs. Let's assume "Billing / Plan" is index 3.
        // The initialization should ideally happen when the "Payment Methods" sub-tab is selected.
        // For now, if the main "Billing / Plan" tab (index 3) is selected on first render, we initialize.
        // A more robust solution would involve handling the RadzenTabs Change event for the *inner* tabs.
        if (firstRender && tabSelected == 3)
        {
            Console.WriteLine("OnAfterRenderAsync: Billing/Plan tab selected on first render. Attempting to initialize Stripe.");
            await InitializeStripe();
        }
        // If the tab changes, we might also need to initialize Stripe.
        // This requires handling the OnChange event of the main RadzenTabs and then checking the sub-tab.
        // For simplicity, this example only initializes on first render if the tab is already selected.
        // A better approach would be to call InitializeStripe() when the "Payment Methods" sub-tab is specifically activated.
    }

    // Consider adding a method to be called by the OnChange event of the *outer* RadzenTabs
    // And then, if the "Billing / Plan" tab is selected, and if there are inner tabs,
    // find a way to trigger InitializeStripe when the "Payment Methods" inner tab is selected.
    // For example, the OnChange event for the outer tabs:
    /*
    protected async Task OnMainTabChange(int index)
    {
        tabSelected = index; // Update the current main tab index
        if (tabSelected == 3) // "Billing / Plan" tab
        {
            // If the "Payment Methods" sub-tab is immediately visible or becomes visible, initialize Stripe.
            // This might require knowing the state of the inner RadzenTabs.
            // For now, we assume InitializeStripe() is called if the main tab is 3.
            // If the payment methods tab is the default first sub-tab, this might be okay.
            Console.WriteLine("OnMainTabChange: Billing/Plan tab (index 3) selected. Initializing Stripe.");
            await InitializeStripe();
        }
    }
    */
    // Make sure the `OnChange` on the main `<RadzenTabs>` in AccountSettings.razor is correctly wired up
    // e.g., `@bind-SelectedIndex="tabSelected" Change="@OnMainTabChange"`
    // The existing `OnChange` in the .razor file is for the main tabs, so it can be updated.

}
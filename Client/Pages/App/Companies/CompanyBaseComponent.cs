using Microsoft.AspNetCore.Components;
using MyVideoResume.Client.Services;

namespace MyVideoResume.Client.Pages.App.Companies;

public class CompanyBaseComponent : BaseComponent
{
    [Inject] protected CompanyWebService WebService { get; set; }

}

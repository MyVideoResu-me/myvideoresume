using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using MyVideoResume.Client.Services;
using MyVideoResume.Services;
using MyVideoResume.Client.Shared.Security.Recaptcha;
using MyVideoResume.Web.Common;
//using Refit;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using MyVideoResume.Client.Services.FeatureFlag;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorBootstrap();
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MyVideoResumeTheme";
    options.Duration = TimeSpan.FromDays(365);
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<FeatureFlagClientService>();
builder.Services.AddSingleton<RecaptchaService>();
//builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<DataContextService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddHttpClient("MyVideoResume.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
//builder.Services.AddHeaderPropagation(options => options.Headers.Add("Authorization"));
//builder.Services.AddRefitClient<IMyVideoResumeApi>()
//                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() { AllowAutoRedirect = false })
//                .ConfigureHttpClient(client =>
//                {
//                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
//                })
//                .AddHeaderPropagation();
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.HttpClientFactory));
builder.Services.AddScoped<FeatureFlagWebService>();
builder.Services.AddScoped<SecurityWebService>();
builder.Services.AddScoped<DashboardWebService>();
builder.Services.AddScoped<ResumeWebService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();
var host = builder.Build();
await host.RunAsync();
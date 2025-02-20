using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyVideoResume.AI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Register the PredictionEnginePool as a service in the IoC container for DI
builder.Services.AddScoped<PromptEngine>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient("MyVideoResume", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MyVideoResume"));
var host = builder.Build();
await host.RunAsync();
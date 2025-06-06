using Radzen;
using MyVideoResume.Server.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using MyVideoResume.Server.Data;
using Microsoft.AspNetCore.Identity;
using MyVideoResume.Data.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;
using IdentityModel;
using MyVideoResume.ML.SentimentAnalysis;
using Blazored.LocalStorage;
using MyVideoResume.Client.Services;
using MyVideoResume.Services;
using MyVideoResume.AI;
using MyVideoResume.Documents;
using MyVideoResume.Application.Resume;
using MyVideoResume.Application;
using Scalar.AspNetCore;
using MyVideoResume.Workers;
using MyVideoResume.Application.FeatureFlag;
using MyVideoResume.Client.Services.FeatureFlag;
using MyVideoResume.Application.Job;
using AutoMapper;
using MyVideoResume.Mapper;
using MyVideoResume.Data.Models.Account;
using Account = MyVideoResume.Application.Account;
using MyVideoResume.Client.Pages.Shared.Security.Recaptcha;
using MyVideoResume.Application.Job.BackgroundProcessing;
using MyVideoResume.Application.Payments;
using Stripe;
using MyVideoResume.Application.DataCollection;
using UAParser;
using MyVideoResume.Application.Productivity;

var builder = WebApplication.CreateBuilder(args);
//Logging
var loggingConnectionString = builder.Configuration.GetConnectionString("Logging");
var newRelicKey = builder.Configuration.GetValue<string>("NewRelic:LoggingKey");
var loggerConfiguration = new LoggerConfiguration()
#if DEBUG
.MinimumLevel.Debug()
#else
.MinimumLevel.Warning()
#endif

.Enrich.FromLogContext()

#if RELEASE
.WriteTo.NewRelicLogs(endpointUrl: "https://log-api.newrelic.com/log/v1", applicationName: "MyVideoResu.ME-WEB", licenseKey: newRelicKey)
#else
.WriteTo.Async(c => c.Console())
.WriteTo.Async(c => c.File($"Logs/logs{DateTime.Now.ToEpochTime()}.txt"))
//.WriteTo.MSSqlServer(connectionString: loggingConnectionString, sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
#endif
;

Log.Logger = loggerConfiguration.CreateLogger();


builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024)
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddControllers();

builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddRadzenComponents();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = "MyVideoResumeTheme";
    options.Duration = TimeSpan.FromDays(365);
});

builder.Services.AddScoped<DataContextService>();
builder.Services.AddDbContext<MyVideoResume.Data.DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddControllers().AddOData(opt =>
{
    var oDataBuilderDataContext = new ODataConventionModelBuilder();
    opt.AddRouteComponents("odata/DataContext", oDataBuilderDataContext.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



builder.Services.AddScoped<Account.AccountService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddSingleton<DocumentProcessor>();
builder.Services.AddSingleton<RecaptchaService>();

#if DEBUG
builder.Services.AddSingleton<IEmailService, EmailService>();
#else
builder.Services.AddSingleton<IEmailService, ZohoEmailService>();
#endif

builder.Services.AddScoped<AccountWebService>();
builder.Services.AddScoped<CompanyWebService>();
builder.Services.AddScoped<JobWebService>();
builder.Services.AddSingleton<IJobPromptEngine, JobPromptEngine>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ResumeWebService>();
builder.Services.AddSingleton<IResumePromptEngine, ResumePromptEngine>();
builder.Services.AddScoped<ResumeService>();
builder.Services.AddScoped<ProductivityService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddSingleton<ResumeBackgroundJobService>();
builder.Services.AddSingleton<JobQueueProcessor>();
builder.Services.AddSingleton<JobRecommendationProcessor>();
builder.Services.AddSingleton<JobWebsiteProcessor>();
builder.Services.AddSingleton<IFeatureFlagService, SplitFeatureFlagService>();
builder.Services.AddScoped<FeatureFlagClientService>();
builder.Services.AddScoped<FeatureFlagWebService>();
builder.Services.AddScoped<SecurityWebService>();
builder.Services.AddScoped<DashboardWebService>();
builder.Services.AddScoped<ProductivityWebService>();
builder.Services.AddScoped<MatchWebService>();
builder.Services.AddScoped<NotificationWebService>();
builder.Services.AddHttpClient("MyVideoResume").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = true }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
builder.Services.AddAuthentication();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<Parser>(Parser.GetDefault());

// Register other services
builder.Services.AddTransient<IRequestLogger, RequestLogger>();

// Application Services
builder.Services.AddScoped<IPaymentService, PaymentService>(); // Added IPaymentService registration

//Payments
builder.Services.Configure<StripeConfig>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["ApiKey"];
StripeConfiguration.ClientId = builder.Configuration.GetSection("Stripe")["ClientId"];

//AI & ML 
builder.Services.AddSentimentAnalysis(builder);
builder.Services.AddAIPromptEngine(builder);

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers().AddOData(o =>
{
    var oDataBuilder = new ODataConventionModelBuilder();
    oDataBuilder.EntitySet<ApplicationUser>("ApplicationUsers");
    var usersType = oDataBuilder.StructuralTypes.First(x => x.ClrType == typeof(ApplicationUser));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.Password)));
    usersType.AddProperty(typeof(ApplicationUser).GetProperty(nameof(ApplicationUser.ConfirmPassword)));
    oDataBuilder.EntitySet<ApplicationRole>("ApplicationRoles");
    o.AddRouteComponents("odata/Identity", oDataBuilder.GetEdmModel()).Count().Filter().OrderBy().Expand().Select().SetMaxTop(null).TimeZone = TimeZoneInfo.Utc;
});
builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();

builder.Services.AddWorkers(builder.Configuration);

var mapperConfiguration = new MapperConfiguration(configuration =>
{
    var profile = new MappingProfile();
    configuration.AddProfile(profile);
});
var mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Host.UseSerilog();



var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(state =>
        {
            var httpcontext = (HttpContext)state;
            httpcontext.Response.Headers.Remove("Content-Security-Policy");
            httpcontext.Response.Headers.Add("Content-Security-Policy", "frame-ancestors hirefractionaltalent.com *.hirefractionaltalent.com https:;");
            return Task.CompletedTask;
        }, context);
        await next();
    });

    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSerilogRequestLogging();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseWorkers(builder.Configuration);
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MyVideoResume.Client._Imports).Assembly);
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();
app.Run();
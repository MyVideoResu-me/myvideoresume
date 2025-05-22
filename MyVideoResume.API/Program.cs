using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyVideoResume.Application.Job;
using MyVideoResume.Data.Models;
using MyVideoResume.Data.Models.Account;
using MyVideoResume.Server.Data;
using System.Text;
using Serilog;
using IdentityModel;
using MyVideoResume.Documents;
using MyVideoResume.Application.Account;
using AutoMapper;
using MyVideoResume.Mapper;
using MyVideoResume.Application.Resume;
using MyVideoResume.Application;
using MyVideoResume.Application.Productivity;
using Microsoft.OpenApi.Models;

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
.WriteTo.NewRelicLogs(endpointUrl: "https://log-api.newrelic.com/log/v1", applicationName: "MyVideoResu.ME-API", licenseKey: newRelicKey)
#else
.WriteTo.Async(c => c.Console())
.WriteTo.Async(c => c.File($"Logs/logs{DateTime.Now.ToEpochTime()}.txt"))
//.WriteTo.MSSqlServer(connectionString: loggingConnectionString, sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
#endif
;

Log.Logger = loggerConfiguration.CreateLogger();

builder.Services.AddDbContext<MyVideoResume.Data.DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationIdentityDbContext>().AddDefaultTokenProviders();
//builder.Services.AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddScoped<ResumeService>();
builder.Services.AddScoped<ProductivityService>();
builder.Services.AddScoped<MatchService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<DocumentProcessor>();
builder.Services.AddScoped<IJobPromptEngine, JobPromptEngine>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<Processor>();
builder.Services.AddHttpClient("MyVideoResume").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = true }).AddHeaderPropagation(o => o.Headers.Add("Cookie"));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
});

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this line
app.UseAuthorization();

app.MapControllers();
app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>().Database.Migrate();

app.Run();
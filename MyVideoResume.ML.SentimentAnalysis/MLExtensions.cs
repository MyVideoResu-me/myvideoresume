using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using MyVideoResume.ML.SentimentAnalysis.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.ML.SentimentAnalysis;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSentimentAnalysis(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var directoryName = System.IO.Path.GetDirectoryName(assemblyLocation);
        var modelPath = builder.Configuration["AI:MLModelFilePath"];
        var path = directoryName != null && modelPath != null 
            ? System.IO.Path.Combine(directoryName, modelPath)
            : null;

        services.AddPredictionEnginePool<SampleObservation, SamplePrediction>()
        .FromFile(path);

        services
           .AddControllers()
           // Notice the assembly is the type of this class, as this
           // is the assembly the controller is in.
           // You'll have to call this for every assembly you have
           // controllers in, except for any controllers
           // you might put in your worker service project.
           .AddApplicationPart(typeof(IServiceCollectionExtensions).Assembly);

        return services;
    }
}

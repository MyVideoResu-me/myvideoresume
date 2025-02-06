using Microsoft.Extensions.DependencyInjection;
using MyVideoResume.Abstractions.DataCollection;
using MyVideoResume.Data;
using MyVideoResume.Data.Models.DataCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Application.DataCollection;

public interface IRequestLogger
{
    Task LogRequestAsync(RequestLogEntity requestLog);
}

public class RequestLogger : IRequestLogger
{
    private readonly IServiceProvider _provider;

    public RequestLogger(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task LogRequestAsync(RequestLogEntity requestLog)
    {
        using (var scope = _provider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            // Log the request asynchronously without blocking the main thread
            await _dbContext.RequestLogs.AddAsync(requestLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}


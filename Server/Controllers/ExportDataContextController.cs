using MyVideoResume.Data;
using MyVideoResume.Services;
using MyVideoResume.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace MyVideoResume.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public partial class ExportDataContextController : ExportController
{
    private readonly DataContext context;
    private readonly DataContextService service;

    public ExportDataContextController(DataContext context, DataContextService service)
    {
        this.service = service;
        this.context = context;
    }

    [HttpGet("csv")]
    public override FileStreamResult ToCSV(IQueryable query, string fileName = null)
    {
        return base.ToCSV(query, fileName);
    }

    [HttpGet("excel")]
    public override FileStreamResult ToExcel(IQueryable query, string fileName = null)
    {
        return base.ToExcel(query, fileName);
    }
}

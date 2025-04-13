using Microsoft.AspNetCore.Components;
using MyVideoResume.Abstractions.Productivity;
using MyVideoResume.Client.Services;

namespace MyVideoResume.Client.Pages.App.Tasks;

public class TaskBaseComponent : BaseComponent
{
    [Inject] protected ProductivityWebService Service { get; set; }

    protected List<TaskDTO> Items { get; set; } = new List<TaskDTO>();
    protected IList<TaskDTO> SelectedItems { get; set; } = new List<TaskDTO>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
        {
            Items = await Service.TasksRead();
        }
    }
}

using Microsoft.AspNetCore.Components;
using MyVideoResume.Abstractions.Core;
using MyVideoResume.Abstractions.Productivity;
using MyVideoResume.Client.Services;
using MyVideoResume.Extensions;
using MyVideoResume.Web.Common;
using Radzen;

namespace MyVideoResume.Client.Pages.App.Tasks;

public class TaskBaseComponent : BaseComponent
{
    [Inject] protected ProductivityWebService Service { get; set; }
    [Parameter] public EventCallback<ResponseResult> Deleted { get; set; }

    protected List<IProductivityItem> Items { get; set; } = new List<IProductivityItem>();
    protected IList<IProductivityItem> SelectedItems { get; set; } = new List<IProductivityItem>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Security.IsAuthenticated())
        {
            Items = await Service.TasksRead();
        }
    }

    protected bool DisplayItem(IProductivityItem item)
    {
        var result = false;
        result = item.CreatedByUserId == Security.User.Id;
        return result;
    }

    protected async Task ViewAction(IProductivityItem item)
    {
        NavigateTo(Paths.Tasks_View, item.Id);
    }

    protected async Task EditAction(IProductivityItem item)
    {
        NavigateTo("job/builder", item.Id);
    }


    protected async Task DeleteAction(IProductivityItem item)
    {
        NavigateTo("job/builder", item.Id);
    }
    protected async Task ShowNotification(ResponseResult result)
    {
        if (result.ErrorMessage.HasValue())
        {
            ShowErrorNotification("Failed to Delete", string.Empty);
        }
        else
        {
            ShowSuccessNotification("Job Deleted", string.Empty);
            Items = await Service.TasksRead();
        }
    }
}

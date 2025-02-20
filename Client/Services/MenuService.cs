namespace MyVideoResume.Client.Services;

public partial class MenuService
{
    public bool SidebarExpanded { get; set; } = true;

    public void SidebarToggleClick()
    {
        SidebarExpanded = !SidebarExpanded;
    }
}
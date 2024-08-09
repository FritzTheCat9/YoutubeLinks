using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzBreadcrumbs : ComponentBase
    {
        [Parameter] public List<BreadcrumbItem> Items { get; set; }
    }
}
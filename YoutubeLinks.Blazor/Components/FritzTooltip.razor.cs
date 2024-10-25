using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzTooltip : ComponentBase
{
    [Parameter] public string Text { get; set; }
    [Parameter] public Color Color { get; set; } = Color.Primary;
    [Parameter] public RenderFragment ChildContent { get; set; }
}
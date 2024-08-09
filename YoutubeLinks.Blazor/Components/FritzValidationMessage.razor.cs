using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace YoutubeLinks.Blazor.Components
{
    public partial class FritzValidationMessage<T> : ComponentBase
    {
        [Parameter] public Expression<Func<T>> For { get; set; }
    }
}
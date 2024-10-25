using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace YoutubeLinks.Blazor.Components;

public partial class FritzValidationMessage<T> : ComponentBase
{
    [Parameter] public Expression<Func<T>> For { get; set; }
}
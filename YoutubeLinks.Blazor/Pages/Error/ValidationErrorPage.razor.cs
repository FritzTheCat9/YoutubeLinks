using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Blazor.Exceptions;

namespace YoutubeLinks.Blazor.Pages.Error;

public partial class ValidationErrorPage(
    ValidationErrors validationErrors,
    IStringLocalizer<App> localizer)
    : ComponentBase { }
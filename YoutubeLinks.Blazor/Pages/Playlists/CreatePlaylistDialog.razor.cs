using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Sdk.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.Blazor.Pages.Playlists;

public partial class CreatePlaylistDialog(
    IExceptionHandler exceptionHandler,
    IPlaylistApiClient playlistApiClient,
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    private CustomValidator _customValidator;

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }
    [Parameter] public CreatePlaylist.Command Command { get; set; } = new();

    private async Task HandleValidSubmit()
    {
        try
        {
            await playlistApiClient.CreatePlaylist(Command);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (MyValidationException validationException)
        {
            _customValidator.DisplayErrors(validationException.Errors);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
    }

    public abstract class CreatePlaylistDialogConst
    {
        public const string NameInput = "create-playlist-dialog-name-input";
        public const string PublicCheckbox = "create-playlist-dialog-public-checkbox";
        public const string CreatePlaylistButton = "create-playlist-dialog-create-playlist-button";
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Shared.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.Blazor.Pages.Playlists;

public partial class UpdatePlaylistDialog(
    IExceptionHandler exceptionHandler,
    IPlaylistApiClient playlistApiClient,
    IStringLocalizer<App> localizer)
    : ComponentBase
{
    private CustomValidator _customValidator;

    [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
    [Parameter] public UpdatePlaylist.Command Command { get; set; } = new();

    private async Task HandleValidSubmit()
    {
        try
        {
            await playlistApiClient.UpdatePlaylist(Command);
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

    public abstract class UpdatePlaylistDialogConst
    {
        public const string NameInput = "update-playlist-dialog-name-input";
        public const string PublicCheckbox = "update-playlist-dialog-public-checkbox";
        public const string UpdatePlaylistButton = "update-playlist-dialog-update-playlist-button";
    }
}
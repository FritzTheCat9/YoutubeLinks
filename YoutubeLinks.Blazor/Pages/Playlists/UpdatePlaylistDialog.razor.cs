using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Pages.Error;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.Blazor.Pages.Playlists
{
    public partial class UpdatePlaylistDialog : ComponentBase
    {
        private CustomValidator _customValidator;

        public class UpdatePlaylistDialogConst
        {
            public const string NameInput = "update-playlist-dialog-name-input";
            public const string PublicCheckbox = "update-playlist-dialog-public-checkbox";
            public const string UpdatePlaylistButton = "update-playlist-dialog-update-playlist-button";
        }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        [Parameter] public UpdatePlaylist.Command Command { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
                await PlaylistApiClient.UpdatePlaylist(Command);
                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (MyValidationException validationException)
            {
                _customValidator.DisplayErrors(validationException.Errors);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
        }
    }
}
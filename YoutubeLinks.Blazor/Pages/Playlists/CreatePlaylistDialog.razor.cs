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
    public partial class CreatePlaylistDialog : ComponentBase
    {
        private CustomValidator _customValidator;

        public class CreatePlaylistDialogConst
        {
            public const string NameInput = "create-playlist-dialog-name-input";
            public const string PublicCheckbox = "create-playlist-dialog-public-checkbox";
            public const string CreatePlaylistButton = "create-playlist-dialog-create-playlist-button";
        }

        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }

        [Parameter] public CreatePlaylist.Command Command { get; set; } = new();

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        private async Task HandleValidSubmit()
        {
            try
            {
                await PlaylistApiClient.CreatePlaylist(Command);
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
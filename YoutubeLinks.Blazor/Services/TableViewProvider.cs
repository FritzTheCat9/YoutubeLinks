using Blazored.LocalStorage;

namespace YoutubeLinks.Blazor.Services
{
    public interface ITableViewProvider
    {
        Task<bool> GetTableView();
        Task SetTableView(bool value);
    }

    public class TableViewProvider : ITableViewProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private const string TableView = "TableView";

        public TableViewProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> GetTableView()
            => await _localStorageService.GetItemAsync<bool?>(TableView) ?? true;

        public async Task SetTableView(bool value)
            => await _localStorageService.SetItemAsync(TableView, value);
    }
}

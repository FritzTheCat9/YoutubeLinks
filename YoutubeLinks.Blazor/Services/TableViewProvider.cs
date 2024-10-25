using Blazored.LocalStorage;

namespace YoutubeLinks.Blazor.Services;

public interface ITableViewProvider
{
    Task<bool> GetTableView();
    Task SetTableView(bool value);
}

public class TableViewProvider(ILocalStorageService localStorageService) : ITableViewProvider
{
    private const string TableView = "TableView";

    public async Task<bool> GetTableView()
        => await localStorageService.GetItemAsync<bool?>(TableView) ?? true;

    public async Task SetTableView(bool value)
        => await localStorageService.SetItemAsync(TableView, value);
}
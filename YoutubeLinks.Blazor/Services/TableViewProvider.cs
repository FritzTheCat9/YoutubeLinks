using Blazored.LocalStorage;

namespace YoutubeLinks.Blazor.Services;

public interface ITableViewProvider
{
    Task<bool> GetTableView();
    Task SetTableView(bool value);
}

public class TableViewProvider(ILocalStorageService localStorageService) : ITableViewProvider
{
    private const string _tableView = "TableView";

    public async Task<bool> GetTableView()
    {
        return await localStorageService.GetItemAsync<bool?>(_tableView) ?? true;
    }

    public async Task SetTableView(bool value)
    {
        await localStorageService.SetItemAsync(_tableView, value);
    }
}
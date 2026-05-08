using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeGame.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LifeGame.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "LifeGame";

    [ObservableProperty]
    private bool _hasSavedGame;

    [ObservableProperty]
    private int _totalGames;

    [ObservableProperty]
    private int _completedGames;

    private readonly Services.ArchiveService _archiveService;

    public MainViewModel(Services.ArchiveService archiveService)
    {
        _archiveService = archiveService;
        _ = LoadStatsAsync();
    }

    private async Task LoadStatsAsync()
    {
        var records = await _archiveService.GetAllRecordsAsync();
        TotalGames = records.Count;
        CompletedGames = records.Count(r => r.IsCompleted);
        HasSavedGame = records.Any(r => !r.IsCompleted);
    }

    public IRelayCommand StartNewGameCommand => new RelayCommand(() =>
    {
        App.ServiceProvider.GetRequiredService<MainWindow>().NavigateTo(
            App.ServiceProvider.GetRequiredService<GameViewModel>());
    });

    public IRelayCommand ContinueCommand => new RelayCommand(() =>
    {
        App.ServiceProvider.GetRequiredService<MainWindow>().NavigateTo(
            App.ServiceProvider.GetRequiredService<GameViewModel>());
    });

    public IRelayCommand HistoryCommand => new RelayCommand(() =>
    {
        App.ServiceProvider.GetRequiredService<MainWindow>().NavigateTo(
            App.ServiceProvider.GetRequiredService<HistoryViewModel>());
    });

    public IRelayCommand SettingsCommand => new RelayCommand(() =>
    {
        App.ServiceProvider.GetRequiredService<MainWindow>().NavigateTo(
            App.ServiceProvider.GetRequiredService<SettingsViewModel>());
    });

    public IRelayCommand RefreshCommand => new RelayCommand(async () => await LoadStatsAsync());
}

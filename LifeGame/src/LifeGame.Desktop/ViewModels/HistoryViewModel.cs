using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeGame.Core.Models;
using LifeGame.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LifeGame.Desktop.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly Services.ArchiveService _archiveService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private GameRecord? _selectedRecord;

    [ObservableProperty]
    private bool _showDetail;

    [ObservableProperty]
    private string _filterEndingType = "全部";

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _completedCount;

    [ObservableProperty]
    private Dictionary<string, int> _endingStats = new();

    public ObservableCollection<GameRecord> Records { get; } = new();

    public ObservableCollection<string> FilterOptions { get; } = new() { "全部", "已完成", "未完成" };

    public HistoryViewModel(Services.ArchiveService archiveService)
    {
        _archiveService = archiveService;
        _ = LoadRecordsAsync();
    }

    [RelayCommand]
    private async Task LoadRecordsAsync()
    {
        IsLoading = true;
        try
        {
            Records.Clear();
            var records = await _archiveService.GetAllRecordsAsync();
            foreach (var record in records)
            {
                Records.Add(record);
            }

            TotalCount = records.Count;
            CompletedCount = records.Count(r => r.IsCompleted);
            EndingStats = await _archiveService.GetEndingStatsAsync();

            if (!FilterOptions.Contains("结局"))
            {
                FilterOptions.Add("结局");
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ViewDetail(GameRecord record)
    {
        SelectedRecord = record;
        ShowDetail = true;
    }

    [RelayCommand]
    private void CloseDetail()
    {
        ShowDetail = false;
        SelectedRecord = null;
    }

    [RelayCommand]
    private void ContinueGame(GameRecord record)
    {
        App.ServiceProvider.GetRequiredService<Services.GameService>().LoadGame(record);
        App.ServiceProvider.GetRequiredService<MainWindow>()
            .NavigateTo(App.ServiceProvider.GetRequiredService<GameViewModel>());
    }

    [RelayCommand]
    private async Task DeleteRecordAsync(GameRecord record)
    {
        await _archiveService.DeleteRecordAsync(record.RecordId);
        Records.Remove(record);
        TotalCount--;
        if (record.IsCompleted) CompletedCount--;
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(GameRecord record)
    {
        await _archiveService.ToggleFavoriteAsync(record.RecordId);
        record.IsFavorite = !record.IsFavorite;
        var index = Records.IndexOf(record);
        if (index >= 0)
        {
            Records.RemoveAt(index);
            Records.Insert(record.IsFavorite ? 0 : index, record);
        }
    }

    [RelayCommand]
    private void Back()
    {
        App.ServiceProvider.GetRequiredService<MainWindow>()
            .NavigateTo(App.ServiceProvider.GetRequiredService<MainViewModel>());
    }
}

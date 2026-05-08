using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeGame.Core.Models;
using LifeGame.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LifeGame.Desktop.ViewModels;

public partial class GameViewModel : ObservableObject
{
    private readonly Services.GameService _gameService;
    private readonly Services.ArchiveService _archiveService;

    [ObservableProperty]
    private string _storyContent = string.Empty;

    [ObservableProperty]
    private string _currentLocation = string.Empty;

    [ObservableProperty]
    private int _currentDepth;

    [ObservableProperty]
    private int _totalTime;

    [ObservableProperty]
    private string _gameTitle = string.Empty;

    [ObservableProperty]
    private bool _isGameOver;

    [ObservableProperty]
    private string _endingTitle = string.Empty;

    [ObservableProperty]
    private string _endingType = string.Empty;

    [ObservableProperty]
    private bool _showChoices = true;

    [ObservableProperty]
    private bool _isSaving;

    public ObservableCollection<ChoiceItem> Choices { get; } = new();

    public GameViewModel(Services.GameService gameService, Services.ArchiveService archiveService)
    {
        _gameService = gameService;
        _archiveService = archiveService;

        _gameService.OnNodeChanged += OnNodeChanged;
        _gameService.OnGameEnded += OnGameEnded;

        Initialize();
    }

    private void Initialize()
    {
        var storyTree = _gameService.CurrentStoryTree;
        var node = _gameService.CurrentNode;

        if (storyTree != null)
        {
            GameTitle = storyTree.Title;
        }

        if (node != null)
        {
            StoryContent = node.Content;
            CurrentDepth = node.Depth;
            ShowChoices = !node.IsEnding;
            UpdateChoices(node.Choices);
        }

        TotalTime = _gameService.TotalTime;
    }

    private void OnNodeChanged(StoryNode node)
    {
        StoryContent = node.Content;
        CurrentDepth = node.Depth;
        TotalTime = _gameService.TotalTime;
        ShowChoices = !node.IsEnding;
        UpdateChoices(node.Choices);
    }

    private void OnGameEnded(string endingType)
    {
        IsGameOver = true;
        EndingType = endingType;
        var record = _gameService.GetCurrentRecord();
        EndingTitle = record?.EndingTitle ?? "结局";
    }

    private void UpdateChoices(List<StoryChoice> choices)
    {
        Choices.Clear();
        for (int i = 0; i < choices.Count; i++)
        {
            Choices.Add(new ChoiceItem
            {
                Index = i,
                Text = choices[i].Text,
                TimeCost = choices[i].TimeCost
            });
        }
    }

    [RelayCommand]
    private void MakeChoice(ChoiceItem choice)
    {
        _gameService.MakeChoice(choice.Index);
        var isEnding = _gameService.CheckEnding();
        if (!isEnding)
        {
            TotalTime = _gameService.TotalTime;
        }
    }

    [RelayCommand]
    private async Task SaveGameAsync()
    {
        IsSaving = true;
        try
        {
            var record = _gameService.GetCurrentRecord();
            if (record != null)
            {
                await _archiveService.SaveRecordAsync(record);
                Log.Information("游戏已保存: {RecordId}", record.RecordId);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "保存游戏失败");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void BackToMenu()
    {
        _gameService.ClearCurrentGame();
        App.ServiceProvider.GetRequiredService<MainWindow>()
            .NavigateTo(App.ServiceProvider.GetRequiredService<MainViewModel>());
    }

    [RelayCommand]
    private void Restart()
    {
        var storyTree = _gameService.CurrentStoryTree;
        var tags = _gameService.GetCurrentRecord()?.TagList ?? new List<string>();
        var gameName = _gameService.GetCurrentRecord()?.GameName ?? "游戏";

        if (storyTree != null)
        {
            _gameService.StartNewGame(storyTree, gameName + "_重玩", tags);
        }

        IsGameOver = false;
        EndingTitle = string.Empty;
        EndingType = string.Empty;
    }
}

public class ChoiceItem
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public int TimeCost { get; set; }
}

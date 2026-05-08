using System.Text.Json;
using LifeGame.Core.Models;

namespace LifeGame.Desktop.Services;

public class GameService
{
    private StoryTree? _currentStoryTree;
    private StoryNode? _currentNode;
    private GameRecord? _currentRecord;
    private int _totalTime;

    public event Action<StoryNode>? OnNodeChanged;
    public event Action<string>? OnGameEnded;

    public StoryTree? CurrentStoryTree => _currentStoryTree;
    public StoryNode? CurrentNode => _currentNode;
    public GameRecord? CurrentRecord => _currentRecord;
    public int TotalTime => _totalTime;
    public int CurrentDepth => _currentNode?.Depth ?? 0;

    public void StartNewGame(StoryTree storyTree, string gameName, List<string> tags)
    {
        _currentStoryTree = storyTree;
        _currentNode = storyTree.RootNode;
        _totalTime = 0;
        _currentRecord = new GameRecord
        {
            GameName = gameName,
            TagList = tags,
            StoryTreeJson = System.Text.Json.JsonSerializer.Serialize(storyTree),
            Choices = new List<PlayerChoice>()
        };

        OnNodeChanged?.Invoke(_currentNode);
    }

    public async Task StartNewGameAsync(StoryTree storyTree, string gameName, List<string> tags)
    {
        await Task.Run(() => StartNewGame(storyTree, gameName, tags));
    }

    public void LoadGame(GameRecord record)
    {
        _currentRecord = record;
        _currentStoryTree = System.Text.Json.JsonSerializer.Deserialize<StoryTree>(record.StoryTreeJson);

        if (_currentStoryTree == null || _currentStoryTree.AllNodes.Count == 0)
        {
            _currentNode = _currentStoryTree?.RootNode;
        }
        else
        {
            var lastChoice = record.Choices.LastOrDefault();
            if (lastChoice != null)
            {
                _currentNode = _currentStoryTree.AllNodes.Values
                    .FirstOrDefault(n => n.NodeId == lastChoice.NodeId.ToString());
            }

            _currentNode ??= _currentStoryTree.RootNode;
        }

        _totalTime = record.Choices.Sum(c => 5);

        OnNodeChanged?.Invoke(_currentNode!);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (_currentNode == null || _currentStoryTree == null)
            return;

        if (choiceIndex < 0 || choiceIndex >= _currentNode.Choices.Count)
            return;

        var choice = _currentNode.Choices[choiceIndex];

        _currentRecord?.Choices.Add(new PlayerChoice
        {
            NodeId = int.Parse(_currentNode.NodeId),
            ChoiceText = choice.Text
        });

        _totalTime += choice.TimeCost;

        if (_currentStoryTree.AllNodes.TryGetValue(choice.NextNodeId, out var nextNode))
        {
            _currentNode = nextNode;
            OnNodeChanged?.Invoke(_currentNode);
        }
    }

    public bool CheckEnding()
    {
        if (_currentNode == null || !_currentNode.IsEnding)
            return false;

        _currentRecord!.EndingType = _currentNode.EndingType ?? "normal";
        _currentRecord.EndingTitle = _currentNode.EndingTitle ?? "普通结局";
        _currentRecord.EndTime = DateTime.Now;
        _currentRecord.IsCompleted = true;

        OnGameEnded?.Invoke(_currentRecord.EndingType);
        return true;
    }

    public void EndGame(string endingType, string endingTitle)
    {
        if (_currentRecord == null) return;

        _currentRecord.EndingType = endingType;
        _currentRecord.EndingTitle = endingTitle;
        _currentRecord.EndTime = DateTime.Now;
        _currentRecord.IsCompleted = true;

        OnGameEnded?.Invoke(endingType);
    }

    public GameRecord? GetCurrentRecord()
    {
        return _currentRecord;
    }

    public void ClearCurrentGame()
    {
        _currentStoryTree = null;
        _currentNode = null;
        _currentRecord = null;
        _totalTime = 0;
    }
}

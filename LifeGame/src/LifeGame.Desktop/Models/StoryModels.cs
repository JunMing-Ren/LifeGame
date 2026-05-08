namespace LifeGame.Core.Models;

/// <summary>
/// 剧情节点
/// </summary>
public class StoryNode
{
    public int Id { get; set; }

    public string NodeId { get; set; } = Guid.NewGuid().ToString();

    public string Content { get; set; } = string.Empty;

    public List<StoryChoice> Choices { get; set; } = new();

    public bool IsEnding { get; set; }

    public string? EndingType { get; set; }

    public string? EndingTitle { get; set; }

    public int Depth { get; set; }

    public int ApproximateTime { get; set; }
}

/// <summary>
/// 剧情选项
/// </summary>
public class StoryChoice
{
    public string ChoiceId { get; set; } = Guid.NewGuid().ToString();

    public string Text { get; set; } = string.Empty;

    public string NextNodeId { get; set; } = string.Empty;

    public int TimeCost { get; set; }
}

/// <summary>
/// 剧情树
/// </summary>
public class StoryTree
{
    public string Title { get; set; } = string.Empty;

    public string Background { get; set; } = string.Empty;

    public string Setting { get; set; } = string.Empty;

    public StoryNode RootNode { get; set; } = new();

    public Dictionary<string, StoryNode> AllNodes { get; set; } = new();

    public List<string> AvailableEndings { get; set; } = new();
}

/// <summary>
/// 游戏设置
/// </summary>
public class GameSettings
{
    public int Id { get; set; } = 1;

    public string AiProvider { get; set; } = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiEndpoint { get; set; } = "https://api.openai.com/v1";

    public string AiModel { get; set; } = "gpt-4o";

    public double Temperature { get; set; } = 0.8;

    public int MaxTokens { get; set; } = 4000;

    public int TargetDuration { get; set; } = 10;
}

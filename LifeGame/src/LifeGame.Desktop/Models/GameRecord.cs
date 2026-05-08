using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeGame.Core.Models;

/// <summary>
/// 游戏记录实体
/// </summary>
public class GameRecord
{
    [Key]
    public int Id { get; set; }

    public Guid RecordId { get; set; } = Guid.NewGuid();

    [Required]
    public string GameName { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; } = DateTime.Now;

    public DateTime? EndTime { get; set; }

    public string EndingType { get; set; } = string.Empty;

    public string EndingTitle { get; set; } = string.Empty;

    public string PlayerChoices { get; set; } = "[]";

    public string StoryTreeJson { get; set; } = "{}";

    public bool IsCompleted { get; set; }

    public bool IsFavorite { get; set; }

    public string Tags { get; set; } = "[]";

    [NotMapped]
    public List<string> TagList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(Tags) ?? new List<string>();
        set => Tags = System.Text.Json.JsonSerializer.Serialize(value);
    }

    [NotMapped]
    public List<PlayerChoice> Choices
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<PlayerChoice>>(PlayerChoices) ?? new List<PlayerChoice>();
        set => PlayerChoices = System.Text.Json.JsonSerializer.Serialize(value);
    }
}

/// <summary>
/// 玩家选择记录
/// </summary>
public class PlayerChoice
{
    public int NodeId { get; set; }
    public string ChoiceText { get; set; } = string.Empty;
    public DateTime ChoiceTime { get; set; } = DateTime.Now;
}

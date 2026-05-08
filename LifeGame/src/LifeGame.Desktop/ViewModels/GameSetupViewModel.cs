using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeGame.Core.Models;
using LifeGame.Desktop.Services;
using LifeGame.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LifeGame.Desktop.ViewModels;

public partial class GameSetupViewModel : ObservableObject
{
    [ObservableProperty]
    private int _currentStep = 1;

    [ObservableProperty]
    private string _gameName = string.Empty;

    [ObservableProperty]
    private string _selectedGender = string.Empty;

    [ObservableProperty]
    private string _selectedAge = string.Empty;

    [ObservableProperty]
    private string _selectedBackground = string.Empty;

    [ObservableProperty]
    private string _selectedLocation = string.Empty;

    [ObservableProperty]
    private string _selectedTheme = string.Empty;

    [ObservableProperty]
    private string _customTheme = string.Empty;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _generationStatus = "准备生成...";

    [ObservableProperty]
    private double _generationProgress;

    public ObservableCollection<string> GenderOptions { get; } = new()
    {
        "男性", "女性", "其他", "不指定"
    };

    public ObservableCollection<string> AgeOptions { get; } = new()
    {
        "少年 (10-17岁)", "青年 (18-35岁)", 
        "中年 (36-55岁)", "老年 (56岁以上)"
    };

    public ObservableCollection<string> BackgroundOptions { get; } = new()
    {
        "普通家庭", "富裕家庭", "单亲家庭", "军人家庭", "艺术世家", "商业世家"
    };

    public ObservableCollection<string> LocationOptions { get; } = new()
    {
        "繁华都市", "宁静小镇", "偏远乡村", "海滨城市", "山区村落", "边疆地区"
    };

    public ObservableCollection<string> ThemeOptions { get; } = new()
    {
        "悬疑推理", "浪漫爱情", "科幻冒险", "奇幻世界", "职场风云", "校园生活", "自定义"
    };

    public ObservableCollection<string> SelectedTags { get; } = new();

    public bool CanGenerate => !string.IsNullOrEmpty(SelectedGender) &&
                               !string.IsNullOrEmpty(SelectedAge) &&
                               !string.IsNullOrEmpty(SelectedBackground) &&
                               !string.IsNullOrEmpty(SelectedLocation) &&
                               !string.IsNullOrEmpty(SelectedTheme);

    [RelayCommand]
    private void SelectGender(string gender)
    {
        SelectedGender = gender;
        UpdateTags();
        NextStep();
    }

    [RelayCommand]
    private void SelectAge(string age)
    {
        SelectedAge = age;
        UpdateTags();
        NextStep();
    }

    [RelayCommand]
    private void SelectBackground(string background)
    {
        SelectedBackground = background;
        UpdateTags();
        NextStep();
    }

    [RelayCommand]
    private void SelectLocation(string location)
    {
        SelectedLocation = location;
        UpdateTags();
        NextStep();
    }

    [RelayCommand]
    private void SelectTheme(string theme)
    {
        SelectedTheme = theme;
        UpdateTags();
        if (theme != "自定义")
        {
            NextStep();
        }
    }

    [RelayCommand]
    private void ConfirmCustomTheme()
    {
        if (!string.IsNullOrWhiteSpace(CustomTheme))
        {
            SelectedTheme = CustomTheme;
            UpdateTags();
            NextStep();
        }
    }

    private void UpdateTags()
    {
        SelectedTags.Clear();
        if (!string.IsNullOrEmpty(SelectedGender)) SelectedTags.Add(SelectedGender);
        if (!string.IsNullOrEmpty(SelectedAge)) SelectedTags.Add(SelectedAge);
        if (!string.IsNullOrEmpty(SelectedBackground)) SelectedTags.Add(SelectedBackground);
        if (!string.IsNullOrEmpty(SelectedLocation)) SelectedTags.Add(SelectedLocation);
        if (!string.IsNullOrEmpty(SelectedTheme)) SelectedTags.Add(SelectedTheme);
    }

    [RelayCommand]
    private void NextStep()
    {
        if (CurrentStep < 6)
        {
            CurrentStep++;
        }
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentStep > 1)
        {
            CurrentStep--;
        }
    }

    [RelayCommand]
    private async Task GenerateGameAsync()
    {
        if (!CanGenerate) return;

        IsGenerating = true;
        GenerationProgress = 0;
        GenerationStatus = "正在构思剧情...";

        await Task.Delay(500);
        GenerationProgress = 20;
        GenerationStatus = "正在构建人物...";

        await Task.Delay(500);
        GenerationProgress = 40;
        GenerationStatus = "正在设计剧情分支...";

        await Task.Delay(500);
        GenerationProgress = 60;
        GenerationStatus = "正在生成多个结局...";

        await Task.Delay(500);
        GenerationProgress = 80;
        GenerationStatus = "正在完善剧情细节...";

        await Task.Delay(500);
        GenerationProgress = 100;
        GenerationStatus = "生成完成！";

        var storyTree = CreateDemoStoryTree();
        var tags = SelectedTags.ToList();
        var gameName = string.IsNullOrWhiteSpace(GameName) 
            ? $"游戏_{DateTime.Now:yyyyMMdd_HHmmss}" 
            : GameName;

        await App.ServiceProvider.GetRequiredService<Services.GameService>()
            .StartNewGameAsync(storyTree, gameName, tags);

        IsGenerating = false;
        App.ServiceProvider.GetRequiredService<MainWindow>()
            .NavigateTo(App.ServiceProvider.GetRequiredService<GameViewModel>());
    }

    private StoryTree CreateDemoStoryTree()
    {
        var rootNode = new StoryNode
        {
            Id = 1,
            NodeId = "node_1",
            Content = $"你是一个在{SelectedLocation}长大的{SelectedAge.Replace("(", "（").Replace(")", "）").Split('(')[0]}，{SelectedBackground}的{SelectedGender}。\n\n这是一个平凡的早晨，你刚从睡梦中醒来。阳光透过窗帘的缝隙洒进房间，新的一天开始了。\n\n你决定今天要...",
            Depth = 0,
            Choices = new List<StoryChoice>
            {
                new() { ChoiceId = "c1", Text = "出门散步，享受清晨的宁静", NextNodeId = "node_2a", TimeCost = 5 },
                new() { ChoiceId = "c2", Text = "留在家中阅读一本好书", NextNodeId = "node_2b", TimeCost = 5 },
                new() { ChoiceId = "c3", Text = "约朋友一起喝咖啡", NextNodeId = "node_2c", TimeCost = 5 }
            }
        };

        var node2a = new StoryNode
        {
            Id = 2,
            NodeId = "node_2a",
            Content = "你走在清晨的街道上，空气清新，鸟鸣悦耳。\n\n正当你沉浸在美好的晨景中时，一个陌生人突然向你走来，神情焦急...",
            Depth = 1,
            Choices = new List<StoryChoice>
            {
                new() { ChoiceId = "c4", Text = "主动上前询问是否需要帮助", NextNodeId = "node_3a", TimeCost = 5 },
                new() { ChoiceId = "c5", Text = "保持警惕，绕道而行", NextNodeId = "node_3b", TimeCost = 5 },
                new() { ChoiceId = "c6", Text = "假装没看见，继续前行", NextNodeId = "node_3c", TimeCost = 5 }
            }
        };

        var node2b = new StoryNode
        {
            Id = 3,
            NodeId = "node_2b",
            Content = "你从书架上取下一本尘封已久的书，翻开第一页，一张泛黄的照片从中滑落...\n\n照片上的人看起来有些眼熟，你仔细端详着...",
            Depth = 1,
            Choices = new List<StoryChoice>
            {
                new() { ChoiceId = "c7", Text = "询问父母这张照片的来历", NextNodeId = "node_3d", TimeCost = 5 },
                new() { ChoiceId = "c8", Text = "独自调查照片背后的故事", NextNodeId = "node_3e", TimeCost = 5 },
                new() { ChoiceId = "c9", Text = "将照片放回书中，不去深究", NextNodeId = "node_3f", TimeCost = 5 }
            }
        };

        var node2c = new StoryNode
        {
            Id = 4,
            NodeId = "node_2c",
            Content = "你来到约定的咖啡馆，朋友已经等在那里。\n\n但今天的气氛有些不同，朋友看起来心事重重，欲言又止...",
            Depth = 1,
            Choices = new List<StoryChoice>
            {
                new() { ChoiceId = "c10", Text = "耐心等待，让朋友自己开口", NextNodeId = "node_3g", TimeCost = 5 },
                new() { ChoiceId = "c11", Text = "直接询问发生了什么事", NextNodeId = "node_3h", TimeCost = 5 }
            }
        };

        var node3a = new StoryNode
        {
            Id = 5,
            NodeId = "node_3a",
            Content = "你走上前去，陌生人看到你，眼中闪过一丝希望。\n\n\"帮帮我，\"他急切地说，\"我迷路了，而且我的手机没电了...\"\n\n你意识到这可能是一个新的开始...",
            Depth = 2,
            IsEnding = true,
            EndingType = "adventure",
            EndingTitle = "善良的开始",
            Choices = new List<StoryChoice>()
        };

        var node3b = new StoryNode
        {
            Id = 6,
            NodeId = "node_3b",
            Content = "你警惕地绕道而行，陌生人失望地看着你离开。\n\n回到家中，你总觉得心里有些不安，也许你错过了一些重要的事情...",
            Depth = 2,
            IsEnding = true,
            EndingType = "cautious",
            EndingTitle = "谨慎的代价",
            Choices = new List<StoryChoice>()
        };

        var node3c = new StoryNode
        {
            Id = 7,
            NodeId = "node_3c",
            Content = "你假装没看见继续前行，但心里一直惦记着那个陌生人。\n\n也许有一天，你会再次遇到需要帮助的人...但这次，你会做出不同的选择吗？",
            Depth = 2,
            IsEnding = true,
            EndingType = "indifferent",
            EndingTitle = "冷漠的过客",
            Choices = new List<StoryChoice>()
        };

        var node3d = new StoryNode
        {
            Id = 8,
            NodeId = "node_3d",
            Content = "父母看着照片，沉默了很久。最终，他们讲述了那段尘封已久的往事...\n\n原来，你的家族有着不为人知的历史，而你，将继承一段特别的使命。",
            Depth = 2,
            IsEnding = true,
            EndingType = "family",
            EndingTitle = "传承的真相",
            Choices = new List<StoryChoice>()
        };

        var node3e = new StoryNode
        {
            Id = 9,
            NodeId = "node_3e",
            Content = "你开始独自调查，随着线索越来越多，一个惊人的秘密逐渐浮出水面...\n\n这个秘密，将彻底改变你对自己和这个世界的认知。",
            Depth = 2,
            IsEnding = true,
            EndingType = "mystery",
            EndingTitle = "真相的碎片",
            Choices = new List<StoryChoice>()
        };

        var node3f = new StoryNode
        {
            Id = 10,
            NodeId = "node_3f",
            Content = "你将照片放回书中，有些秘密，或许永远不被知道会更好。\n\n你继续过着平凡的生活，但那本书，总是在不经意间出现在你的脑海中...",
            Depth = 2,
            IsEnding = true,
            EndingType = "peaceful",
            EndingTitle = "平凡的幸福",
            Choices = new List<StoryChoice>()
        };

        var node3g = new StoryNode
        {
            Id = 11,
            NodeId = "node_3g",
            Content = "你静静地等待，终于，朋友开口了。\n\n\"其实，我有一个秘密，一直想告诉你...\"\n\n一段新的友谊，或者更特别的关系，就此展开。",
            Depth = 2,
            IsEnding = true,
            EndingType = "friendship",
            EndingTitle = "倾听的友谊",
            Choices = new List<StoryChoice>()
        };

        var node3h = new StoryNode
        {
            Id = 12,
            NodeId = "node_3h",
            Content = "你直接询问，朋友被你的真诚打动，终于说出了心事。\n\n原来，朋友一直承受着巨大的压力，而你的直接反而让他感到轻松。",
            Depth = 2,
            IsEnding = true,
            EndingType = "trust",
            EndingTitle = "真诚的力量",
            Choices = new List<StoryChoice>()
        };

        var allNodes = new Dictionary<string, StoryNode>
        {
            ["node_1"] = rootNode,
            ["node_2a"] = node2a,
            ["node_2b"] = node2b,
            ["node_2c"] = node2c,
            ["node_3a"] = node3a,
            ["node_3b"] = node3b,
            ["node_3c"] = node3c,
            ["node_3d"] = node3d,
            ["node_3e"] = node3e,
            ["node_3f"] = node3f,
            ["node_3g"] = node3g,
            ["node_3h"] = node3h
        };

        return new StoryTree
        {
            Title = $"{SelectedLocation}的{SelectedTheme}人生",
            Background = $"{SelectedBackground}的{SelectedGender}",
            Setting = $"年龄阶段：{SelectedAge}",
            RootNode = rootNode,
            AllNodes = allNodes,
            AvailableEndings = new List<string> { "善良的开始", "谨慎的代价", "冷漠的过客", "传承的真相", "真相的碎片", "平凡的幸福", "倾听的友谊", "真诚的力量" }
        };
    }

    [RelayCommand]
    private void Cancel()
    {
        App.ServiceProvider.GetRequiredService<MainWindow>()
            .NavigateTo(App.ServiceProvider.GetRequiredService<MainViewModel>());
    }
}

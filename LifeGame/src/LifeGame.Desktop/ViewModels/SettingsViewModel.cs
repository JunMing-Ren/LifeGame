using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Serilog;

namespace LifeGame.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _aiProvider = "OpenAI";

    [ObservableProperty]
    private string _apiKey = string.Empty;

    [ObservableProperty]
    private string _apiEndpoint = "https://api.openai.com/v1";

    [ObservableProperty]
    private string _aiModel = "gpt-4o";

    [ObservableProperty]
    private double _temperature = 0.8;

    [ObservableProperty]
    private int _maxTokens = 4000;

    [ObservableProperty]
    private int _targetDuration = 10;

    [ObservableProperty]
    private bool _isSaved;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public string[] AiProviders { get; } = { "OpenAI", "Azure OpenAI", "本地模型" };

    public string[] OpenAIModels { get; } = { "gpt-4o", "gpt-4-turbo", "gpt-3.5-turbo" };

    public string[] AzureModels { get; } = { "gpt-4", "gpt-4-32k", "gpt-35-turbo" };

    [RelayCommand]
    private void SaveSettings()
    {
        try
        {
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LifeGame", "settings.json");

            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);

            var settings = new
            {
                AiProvider,
                ApiKey,
                ApiEndpoint,
                AiModel,
                Temperature,
                MaxTokens,
                TargetDuration
            };

            var json = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(settingsPath, json);

            IsSaved = true;
            StatusMessage = "设置已保存";
            Log.Information("设置已保存");

            Task.Delay(2000).ContinueWith(_ => StatusMessage = string.Empty);
        }
        catch (Exception ex)
        {
            StatusMessage = $"保存失败: {ex.Message}";
            Log.Error(ex, "保存设置失败");
        }
    }

    [RelayCommand]
    private void LoadSettings()
    {
        try
        {
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LifeGame", "settings.json");

            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("aiProvider", out var provider))
                    AiProvider = provider.GetString() ?? "OpenAI";
                if (root.TryGetProperty("apiKey", out var key))
                    ApiKey = key.GetString() ?? string.Empty;
                if (root.TryGetProperty("apiEndpoint", out var endpoint))
                    ApiEndpoint = endpoint.GetString() ?? "https://api.openai.com/v1";
                if (root.TryGetProperty("aiModel", out var model))
                    AiModel = model.GetString() ?? "gpt-4o";
                if (root.TryGetProperty("temperature", out var temp))
                    Temperature = temp.GetDouble();
                if (root.TryGetProperty("maxTokens", out var tokens))
                    MaxTokens = tokens.GetInt32();
                if (root.TryGetProperty("targetDuration", out var duration))
                    TargetDuration = duration.GetInt32();

                Log.Information("设置已加载");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "加载设置失败");
        }
    }

    [RelayCommand]
    private void TestConnection()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            StatusMessage = "请输入API Key";
            return;
        }

        StatusMessage = "正在测试连接...";
        Task.Delay(1000).ContinueWith(_ => StatusMessage = "连接测试功能待实现");
    }

    [RelayCommand]
    private void Back()
    {
        App.ServiceProvider.GetRequiredService<MainWindow>()
            .NavigateTo(App.ServiceProvider.GetRequiredService<MainViewModel>());
    }

    public SettingsViewModel()
    {
        LoadSettings();
    }
}

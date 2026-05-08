using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LifeGame.Desktop.ViewModels;
using LifeGame.Desktop.Services;
using LifeGame.Core.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.IO;

namespace LifeGame;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/lifegame-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("LifeGame 启动");

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LifeGameDbContext>();
            dbContext.Database.EnsureCreated();
            Log.Information("数据库初始化完成");
        }

        var mainWindow = new Desktop.Views.MainWindow();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LifeGame");
        Directory.CreateDirectory(dbPath);

        services.AddDbContext<LifeGameDbContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine(dbPath, "lifegame.db")}"));

        services.AddSingleton<Desktop.Services.GameService>();
        services.AddSingleton<Desktop.Services.ArchiveService>();
        services.AddTransient<Desktop.ViewModels.MainViewModel>();
        services.AddTransient<Desktop.ViewModels.GameViewModel>();
        services.AddTransient<Desktop.ViewModels.HistoryViewModel>();
        services.AddTransient<Desktop.ViewModels.SettingsViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("LifeGame 退出");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}

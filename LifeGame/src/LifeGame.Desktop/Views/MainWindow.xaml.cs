using System.Windows;
using System.Windows.Controls;
using LifeGame.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LifeGame.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NavigateTo(App.ServiceProvider.GetRequiredService<MainViewModel>());
    }

    public void NavigateTo(object viewModel)
    {
        UserControl view = viewModel switch
        {
            MainViewModel => new MainMenuView { DataContext = viewModel },
            GameSetupViewModel => new GameSetupView { DataContext = viewModel },
            GameViewModel => new GameView { DataContext = viewModel },
            HistoryViewModel => new HistoryView { DataContext = viewModel },
            SettingsViewModel => new SettingsView { DataContext = viewModel },
            _ => throw new ArgumentException("Unknown view model type")
        };

        MainContent.Content = view;
    }
}

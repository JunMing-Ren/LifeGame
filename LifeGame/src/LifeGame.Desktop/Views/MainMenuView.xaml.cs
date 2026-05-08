using System.Windows.Controls;
using LifeGame.Desktop.ViewModels;

namespace LifeGame.Desktop.Views;

public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();
        DataContextChanged += (s, e) =>
        {
            if (DataContext is MainViewModel vm)
            {
                UpdateStats(vm);
                vm.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(MainViewModel.TotalGames) ||
                        args.PropertyName == nameof(MainViewModel.CompletedGames))
                    {
                        UpdateStats(vm);
                    }
                };
            }
        };
    }

    private void UpdateStats(MainViewModel vm)
    {
        StatsText.Text = $"总游戏数: {vm.TotalGames} | 已完成: {vm.CompletedGames}";
    }
}

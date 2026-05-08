using System.Windows.Controls;

namespace LifeGame.Desktop.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        ApiKeyBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is ViewModels.SettingsViewModel vm)
            {
                vm.ApiKey = ApiKeyBox.Password;
            }
        };
    }
}
